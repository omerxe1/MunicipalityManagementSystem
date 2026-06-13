using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;
using System.Security.Cryptography;
using System.Text;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Şifre yönetimi controller'ı
    /// - Kendi şifresini değiştirme (tüm adminler)
    /// - Diğer adminlerin şifresini sıfırlama (sadece baş admin)
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class SifreYonetimiController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public SifreYonetimiController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        /// <summary>
        /// Kendi şifresini değiştirme sayfası (Sadece BasAdmin)
        /// </summary>
        [HttpGet]
        public IActionResult SifreDegistir()
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok. Sadece baş admin şifre değiştirebilir.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = rol;
            return View();
        }

        /// <summary>
        /// Kendi şifresini değiştirme işlemi (Sadece BasAdmin)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok. Sadece baş admin şifre değiştirebilir.";
                return RedirectToAction("Index", "Dashboard");
            }

            var kullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                TempData["Error"] = "Oturum bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Index", "Login");
            }

            // Validasyon
            if (string.IsNullOrWhiteSpace(mevcutSifre))
            {
                ViewBag.Error = "Mevcut şifre gereklidir.";
                ViewBag.KullaniciAdi = kullaniciAdi;
                ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                return View();
            }

            if (string.IsNullOrWhiteSpace(yeniSifre) || yeniSifre.Length < 6)
            {
                ViewBag.Error = "Yeni şifre en az 6 karakter olmalıdır.";
                ViewBag.KullaniciAdi = kullaniciAdi;
                ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                return View();
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                ViewBag.Error = "Yeni şifreler eşleşmiyor.";
                ViewBag.KullaniciAdi = kullaniciAdi;
                ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                return View();
            }

            // Kullanıcıyı bul
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.AktifMi);

            if (admin == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Login");
            }

            // Mevcut şifre kontrolü
            var mevcutSifreHash = HashSifre(mevcutSifre);
            if (admin.SifreHash != mevcutSifreHash)
            {
                ViewBag.Error = "Mevcut şifre hatalı.";
                ViewBag.KullaniciAdi = kullaniciAdi;
                ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                return View();
            }

            // Yeni şifreyi hash'le ve kaydet
            admin.SifreHash = HashSifre(yeniSifre);
            _context.Update(admin);
            await _context.SaveChangesAsync();

            // Log kaydı
            await LogAsync("Şifre Değiştirme", "Admin", $"Kullanıcı kendi şifresini değiştirdi: {kullaniciAdi}");

            TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Index", "Dashboard");
        }

        /// <summary>
        /// Diğer adminlerin şifresini sıfırlama sayfası (sadece baş admin)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SifreSifirla()
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok. Sadece baş admin şifre sıfırlayabilir.";
                return RedirectToAction("Index", "Dashboard");
            }

            var admins = await _context.Admins
                .Where(a => a.AktifMi && a.Rol != "BasAdmin") // Baş admin kendi şifresini sıfırlayamaz
                .OrderBy(a => a.KullaniciAdi)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(admins);
        }

        /// <summary>
        /// Diğer adminlerin şifresini sıfırlama işlemi (sadece baş admin)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreSifirla(int adminId, string yeniSifre, string yeniSifreTekrar)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok. Sadece baş admin şifre sıfırlayabilir.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Validasyon
            if (string.IsNullOrWhiteSpace(yeniSifre) || yeniSifre.Length < 6)
            {
                TempData["Error"] = "Yeni şifre en az 6 karakter olmalıdır.";
                return RedirectToAction(nameof(SifreSifirla));
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["Error"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction(nameof(SifreSifirla));
            }

            // Admin'i bul
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(SifreSifirla));
            }

            // Baş admin kendi şifresini sıfırlayamaz
            if (admin.Rol == "BasAdmin")
            {
                TempData["Error"] = "Baş admin şifresi buradan sıfırlanamaz.";
                return RedirectToAction(nameof(SifreSifirla));
            }

            // Şifreyi sıfırla
            admin.SifreHash = HashSifre(yeniSifre);
            _context.Update(admin);
            await _context.SaveChangesAsync();

            // Log kaydı
            var basAdminAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            await LogAsync("Şifre Sıfırlama", "Admin", $"Baş admin {admin.KullaniciAdi} kullanıcısının şifresini sıfırladı. (Baş Admin: {basAdminAdi})");

            TempData["Success"] = $"{admin.KullaniciAdi} kullanıcısının şifresi başarıyla sıfırlandı.";
            return RedirectToAction(nameof(SifreSifirla));
        }

        /// <summary>
        /// Şifre yenileme taleplerini görüntüleme (sadece baş admin)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SifreYenilemeTalepleri()
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok. Sadece baş admin şifre yenileme taleplerini görebilir.";
                return RedirectToAction("Index", "Dashboard");
            }

            var talepler = await _context.SifreYenilemeTalepleri
                .OrderByDescending(t => t.TalepTarihi)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(talepler);
        }

        /// <summary>
        /// Şifre yenileme talebini onaylama ve şifre sıfırlama (sadece baş admin)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TalebiOnayla(int talepId, string yeniSifre, string yeniSifreTekrar)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Validasyon
            if (string.IsNullOrWhiteSpace(yeniSifre) || yeniSifre.Length < 6)
            {
                TempData["Error"] = "Yeni şifre en az 6 karakter olmalıdır.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                TempData["Error"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            // Talebi bul
            var talep = await _context.SifreYenilemeTalepleri.FindAsync(talepId);
            if (talep == null)
            {
                TempData["Error"] = "Talep bulunamadı.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            if (talep.Durum != "Beklemede")
            {
                TempData["Error"] = "Bu talep zaten işleme alınmış.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            // Admin'i bul
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.KullaniciAdi == talep.KullaniciAdi && a.AktifMi);

            if (admin == null)
            {
                talep.Durum = "Reddedildi";
                talep.IslemTarihi = DateTime.Now;
                talep.IslemYapan = HttpContext.Session.GetString("AdminKullaniciAdi");
                _context.Update(talep);
                await _context.SaveChangesAsync();

                TempData["Error"] = "Kullanıcı bulunamadı. Talep reddedildi.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            // Baş admin şifresi buradan sıfırlanamaz
            if (admin.Rol == "BasAdmin")
            {
                talep.Durum = "Reddedildi";
                talep.IslemTarihi = DateTime.Now;
                talep.IslemYapan = HttpContext.Session.GetString("AdminKullaniciAdi");
                _context.Update(talep);
                await _context.SaveChangesAsync();

                TempData["Error"] = "Baş admin şifresi buradan sıfırlanamaz.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            // Şifreyi sıfırla
            admin.SifreHash = HashSifre(yeniSifre);
            _context.Update(admin);

            // Talebi onaylandı olarak işaretle
            talep.Durum = "Onaylandı";
            talep.IslemTarihi = DateTime.Now;
            talep.IslemYapan = HttpContext.Session.GetString("AdminKullaniciAdi");
            _context.Update(talep);

            await _context.SaveChangesAsync();

            // Log kaydı
            var basAdminAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            await LogAsync("Şifre Yenileme Talebi Onaylandı", "Admin", 
                $"Baş admin {admin.KullaniciAdi} kullanıcısının şifre yenileme talebini onayladı ve şifresini sıfırladı. (Baş Admin: {basAdminAdi})");

            TempData["Success"] = $"{admin.KullaniciAdi} kullanıcısının şifre yenileme talebi onaylandı ve şifre sıfırlandı.";
            return RedirectToAction(nameof(SifreYenilemeTalepleri));
        }

        /// <summary>
        /// Şifre yenileme talebini reddetme (sadece baş admin)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TalebiReddet(int talepId, string redNedeni)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "BasAdmin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            var talep = await _context.SifreYenilemeTalepleri.FindAsync(talepId);
            if (talep == null)
            {
                TempData["Error"] = "Talep bulunamadı.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            if (talep.Durum != "Beklemede")
            {
                TempData["Error"] = "Bu talep zaten işleme alınmış.";
                return RedirectToAction(nameof(SifreYenilemeTalepleri));
            }

            talep.Durum = "Reddedildi";
            talep.IslemTarihi = DateTime.Now;
            talep.IslemYapan = HttpContext.Session.GetString("AdminKullaniciAdi");
            _context.Update(talep);
            await _context.SaveChangesAsync();

            // Log kaydı
            var basAdminAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            await LogAsync("Şifre Yenileme Talebi Reddedildi", "Admin", 
                $"Baş admin {talep.KullaniciAdi} kullanıcısının şifre yenileme talebini reddetti. (Baş Admin: {basAdminAdi}, Neden: {redNedeni ?? "Belirtilmedi"})");

            TempData["Success"] = "Talep reddedildi.";
            return RedirectToAction(nameof(SifreYenilemeTalepleri));
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

