using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Services;
using System.Security.Cryptography;
using System.Text;

namespace kocaaliv2.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly KocaaliContext _context;
        private readonly ILoginSecurityService _loginSecurityService;
        private readonly IAdminLogService _adminLogService;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IConfiguration _configuration;

        public LoginController(
            KocaaliContext context,
            ILoginSecurityService loginSecurityService,
            IAdminLogService adminLogService,
            IReCaptchaService reCaptchaService,
            IConfiguration configuration)
        {
            _context = context;
            _loginSecurityService = loginSecurityService;
            _adminLogService = adminLogService;
            _reCaptchaService = reCaptchaService;
            _configuration = configuration;
        }

       
        [HttpGet]
        public IActionResult Index()
        {
            var kullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            if (!string.IsNullOrEmpty(kullaniciAdi))
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
            ViewBag.EnableReCaptcha = _configuration.GetValue<bool>("ReCaptcha:EnableReCaptcha", false);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string kullaniciAdi, string sifre, string? recaptchaToken)
        {
            var clientIP = GetClientIP();
            var enableReCaptcha = _configuration.GetValue<bool>("ReCaptcha:EnableReCaptcha", false);

            // Validasyon kontrolü
            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                ViewBag.Hata = "Kullanıcı adı ve şifre gereklidir.";
                ViewBag.ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
                ViewBag.EnableReCaptcha = enableReCaptcha;
                return View();
            }

            // reCAPTCHA doğrulama (eğer aktifse)
            if (enableReCaptcha)
            {
                if (string.IsNullOrWhiteSpace(recaptchaToken) || !await _reCaptchaService.VerifyAsync(recaptchaToken))
                {
                    ViewBag.Hata = "reCAPTCHA doğrulaması başarısız. Lütfen tekrar deneyin.";
                    ViewBag.ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
                    ViewBag.EnableReCaptcha = enableReCaptcha;
                    return View();
                }
            }

            // Brute Force koruması - Hesap kilitli mi kontrol et
            if (await _loginSecurityService.IsAccountLockedAsync(kullaniciAdi, clientIP))
            {
                ViewBag.Hata = "Çok fazla başarısız giriş denemesi. Hesabınız 15 dakika süreyle kilitlenmiştir.";
                ViewBag.ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
                ViewBag.EnableReCaptcha = enableReCaptcha;
                return View();
            }

            // Kullanıcıyı veritabanından bul
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.AktifMi);

            // Şifre kontrolü (kullanıcı bulunamasa bile aynı mesaj)
            var sifreHash = HashSifre(sifre);
            if (admin == null || admin.SifreHash != sifreHash)
            {
                // Başarısız denemeyi kaydet
                await _loginSecurityService.RecordFailedAttemptAsync(kullaniciAdi, clientIP);
                var remainingAttempts = await _loginSecurityService.GetRemainingAttemptsAsync(kullaniciAdi, clientIP);

                ViewBag.Hata = $"Kullanıcı adı veya şifre hatalı. Kalan deneme hakkı: {remainingAttempts}";
                ViewBag.ReCaptchaSiteKey = _configuration["ReCaptcha:SiteKey"];
                ViewBag.EnableReCaptcha = enableReCaptcha;
                return View();
            }

            // Başarılı giriş - Deneme kayıtlarını temizle
            await _loginSecurityService.ClearFailedAttemptsAsync(kullaniciAdi, clientIP);

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetString("AdminKullaniciAdi", admin.KullaniciAdi);
            HttpContext.Session.SetString("AdminRol", admin.Rol);

            // Giriş işlemini logla
            await _adminLogService.LogAsync(admin.KullaniciAdi, "Giriş", null, "Admin paneline giriş yapıldı", clientIP);

            // Dashboard'a yönlendir
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Kullanıcı çıkış işlemini gerçekleştirir
        /// Session'ı temizler ve Login sayfasına yönlendirir
        /// FAZ 4: Çıkış işlemi loglanır
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var kullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            var clientIP = GetClientIP();

            // Çıkış işlemini logla
            if (!string.IsNullOrEmpty(kullaniciAdi))
            {
                await _adminLogService.LogAsync(kullaniciAdi, "Çıkış", null, "Admin panelinden çıkış yapıldı", clientIP);
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// İstemci IP adresini alır
        /// </summary>
        private string GetClientIP()
        {
            // X-Forwarded-For header'ından IP al (Load Balancer/Proxy için)
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            // X-Real-IP header'ından IP al
            var realIP = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            // Direkt bağlantı IP'si
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Şifreyi SHA256 ile hash'ler
        /// </summary>
        private string HashSifre(string sifre)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(sifre);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}

