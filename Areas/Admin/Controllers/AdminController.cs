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
    /// Admin kullanıcı yönetimi controller'ı
    /// Sadece Admin rolü erişebilir
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class AdminController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public AdminController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService)
        {
            _context = context;
        }

        /// <summary>
        /// Admin kullanıcı listesi
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            var admins = await _context.Admins.OrderBy(a => a.KullaniciAdi).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(admins);
        }

        /// <summary>
        /// Yeni admin kullanıcısı ekleme sayfası
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View();
        }

        /// <summary>
        /// Yeni admin kullanıcısı ekleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(kocaaliv2.Models.Admin admin, string sifre)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Admin model null kontrolü
            if (admin == null)
            {
                ModelState.AddModelError("", "Geçersiz veri gönderildi.");
                ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                return View(new kocaaliv2.Models.Admin());
            }

            // Kullanıcı adı kontrolü
            if (string.IsNullOrWhiteSpace(admin.KullaniciAdi))
            {
                ModelState.AddModelError("KullaniciAdi", "Kullanıcı adı gereklidir.");
            }

            // Şifre kontrolü
            if (string.IsNullOrWhiteSpace(sifre))
            {
                ModelState.AddModelError("", "Şifre gereklidir.");
            }

            // Rol kontrolü - eğer boşsa varsayılan olarak Editor yap
            if (string.IsNullOrWhiteSpace(admin.Rol))
            {
                admin.Rol = "Editor";
            }
            ModelState.Remove("Rol"); // ModelState'den Rol hatasını kaldır
            ModelState.Remove("SifreHash"); // SifreHash form'dan gelmiyor, şifreyi hash'leyerek oluşturuyoruz
            ModelState.Remove("Id"); // Id yeni kayıt için 0 olacak

            // Yeni kullanıcılar varsayılan olarak aktif olmalı
            admin.AktifMi = true;

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(sifre) && !string.IsNullOrWhiteSpace(admin.KullaniciAdi))
            {
                try
                {
                    // Kullanıcı adı tekrar kontrolü
                    var mevcutKullanici = await _context.Admins
                        .FirstOrDefaultAsync(a => a.KullaniciAdi == admin.KullaniciAdi);
                    
                    if (mevcutKullanici != null)
                    {
                        ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten kullanılıyor.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(admin);
                    }

                    // Şifreyi hash'le ve kaydet
                    admin.Id = 0; // Yeni kayıt için Id'yi sıfırla
                    admin.SifreHash = HashSifre(sifre);
                    _context.Admins.Add(admin);
                    var result = await _context.SaveChangesAsync();
                    
                    if (result == 0)
                    {
                        ModelState.AddModelError("", "Kullanıcı kaydedilemedi. Lütfen tekrar deneyin.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(admin);
                    }

                    // FAZ 4: Log kaydı
                    await LogAsync("Ekleme", "Admin", $"Admin kullanıcısı eklendi: {admin.KullaniciAdi} (Rol: {admin.Rol})");

                    TempData["Success"] = "Admin kullanıcısı başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                    // Log the exception
                    await LogAsync("Hata", "Admin", $"Kullanıcı ekleme hatası: {ex.Message}");
                }
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(admin);
        }

        /// <summary>
        /// Admin kullanıcısı düzenleme sayfası
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (id == null) return NotFound();
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(admin);
        }

        /// <summary>
        /// Admin kullanıcısı düzenleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, kocaaliv2.Models.Admin admin, string? yeniSifre)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (id != admin.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Eğer yeni şifre girilmişse, hash'le
                    if (!string.IsNullOrEmpty(yeniSifre))
                    {
                        admin.SifreHash = HashSifre(yeniSifre);
                    }
                    else
                    {
                        // Mevcut şifreyi koru
                        var mevcutAdmin = await _context.Admins.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                        if (mevcutAdmin != null)
                        {
                            admin.SifreHash = mevcutAdmin.SifreHash;
                        }
                    }

                    _context.Update(admin);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    var logMesaj = !string.IsNullOrEmpty(yeniSifre) 
                        ? $"Admin kullanıcısı güncellendi: {admin.KullaniciAdi} (Rol: {admin.Rol}, Şifre değiştirildi)"
                        : $"Admin kullanıcısı güncellendi: {admin.KullaniciAdi} (Rol: {admin.Rol})";
                    await LogAsync("Güncelleme", "Admin", logMesaj);

                    TempData["Success"] = "Admin kullanıcısı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id)) return NotFound();
                    throw;
                }
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(admin);
        }

        /// <summary>
        /// Admin kullanıcısı silme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            var adminKullaniciAdi = admin.KullaniciAdi;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Admin", $"Admin kullanıcısı silindi: {adminKullaniciAdi} (ID: {id})");

            TempData["Success"] = "Admin kullanıcısı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
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

        private bool AdminExists(int id) => _context.Admins.Any(e => e.Id == id);
    }
}

