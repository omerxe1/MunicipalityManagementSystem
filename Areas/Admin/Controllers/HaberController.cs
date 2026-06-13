using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Haber yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class HaberController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        private readonly IFileUploadService _fileUploadService;

        public HaberController(KocaaliContext context, IAdminLogService adminLogService, IFileUploadService fileUploadService)
            : base(adminLogService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Haber listesi sayfası
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var haberler = await _context.Haberler
                .OrderByDescending(h => h.OlusturmaTarihi)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");

            return View(haberler);
        }

        /// <summary>
        /// Yeni haber ekleme sayfası
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View();
        }

        /// <summary>
        /// Yeni haber ekleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Haber haber, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                haber.OlusturmaTarihi = DateTime.Now;
                if (haber.Tarih == default)
                {
                    haber.Tarih = DateTime.Now;
                }

                // Dosya yükleme işlemi
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                        resimDosyasi, 
                        "uploads/haberler",
                        5242880); // 5MB limit

                    if (success && !string.IsNullOrEmpty(fileName))
                    {
                        haber.ResimYolu = "/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(haber);
                    }
                }

                _context.Haberler.Add(haber);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Haber", $"Haber eklendi: {haber.Baslik}");

                TempData["Success"] = "Haber başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(haber);
        }

        /// <summary>
        /// Haber düzenleme sayfası
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var haber = await _context.Haberler.FindAsync(id);
            if (haber == null)
            {
                return NotFound();
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(haber);
        }

        /// <summary>
        /// Haber düzenleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Haber haber, IFormFile? resimDosyasi)
        {
            if (id != haber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Eski resmi al (silme için)
                    var eskiHaber = await _context.Haberler.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
                    var eskiResimYolu = eskiHaber?.ResimYolu;

                    // Yeni dosya yükleme işlemi
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                            resimDosyasi, 
                            "uploads/haberler",
                            5242880); // 5MB limit

                        if (success && !string.IsNullOrEmpty(fileName))
                        {
                            haber.ResimYolu = "/" + fileName;

                            // Eski resmi sil
                            if (!string.IsNullOrEmpty(eskiResimYolu) && eskiResimYolu.StartsWith("/uploads/"))
                            {
                                await _fileUploadService.DeleteFileAsync(eskiResimYolu.TrimStart('/'));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                            return View(haber);
                        }
                    }
                    else if (string.IsNullOrEmpty(haber.ResimYolu) && !string.IsNullOrEmpty(eskiResimYolu))
                    {
                        // Resim yolu temizlendiyse eski resmi sil
                        if (eskiResimYolu.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiResimYolu.TrimStart('/'));
                        }
                    }
                    else if (!string.IsNullOrEmpty(eskiResimYolu) && haber.ResimYolu != eskiResimYolu && !haber.ResimYolu.StartsWith("/uploads/"))
                    {
                        // Manuel URL girildiyse eski yüklenen dosyayı sil
                        if (eskiResimYolu.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiResimYolu.TrimStart('/'));
                        }
                    }

                    _context.Update(haber);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Haber", $"Haber güncellendi: {haber.Baslik} (ID: {haber.Id})");

                    TempData["Success"] = "Haber başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HaberExists(haber.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(haber);
        }

        /// <summary>
        /// Haber silme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var haber = await _context.Haberler.FindAsync(id);
            if (haber == null)
            {
                return NotFound();
            }

            var haberBaslik = haber.Baslik; // Silmeden önce başlığı kaydet

            _context.Haberler.Remove(haber);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Haber", $"Haber silindi: {haberBaslik} (ID: {id})");

            TempData["Success"] = "Haber başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Haber var mı kontrolü
        /// </summary>
        private bool HaberExists(int id)
        {
            return _context.Haberler.Any(e => e.Id == id);
        }
    }
}

