using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Etkinlik yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class EtkinlikController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        private readonly IFileUploadService _fileUploadService;

        public EtkinlikController(KocaaliContext context, IAdminLogService adminLogService, IFileUploadService fileUploadService)
            : base(adminLogService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Etkinlik listesi sayfası
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var etkinlikler = await _context.Etkinlikler
                .OrderByDescending(e => e.BaslangicTarihi)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");

            return View(etkinlikler);
        }

        /// <summary>
        /// Yeni etkinlik ekleme sayfası
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View();
        }

        /// <summary>
        /// Yeni etkinlik ekleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Etkinlik etkinlik, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (etkinlik.BaslangicTarihi == default)
                {
                    etkinlik.BaslangicTarihi = DateTime.Now;
                }
                if (etkinlik.Tarih == default)
                {
                    etkinlik.Tarih = etkinlik.BaslangicTarihi;
                }

                // Dosya yükleme işlemi
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                        resimDosyasi, 
                        "uploads/etkinlikler",
                        5242880); // 5MB limit

                    if (success && !string.IsNullOrEmpty(fileName))
                    {
                        etkinlik.ResimYolu = "/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(etkinlik);
                    }
                }

                _context.Etkinlikler.Add(etkinlik);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Etkinlik", $"Etkinlik eklendi: {etkinlik.Baslik}");

                TempData["Success"] = "Etkinlik başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(etkinlik);
        }

        /// <summary>
        /// Etkinlik düzenleme sayfası
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null)
            {
                return NotFound();
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(etkinlik);
        }

        /// <summary>
        /// Etkinlik düzenleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Etkinlik etkinlik, IFormFile? resimDosyasi)
        {
            if (id != etkinlik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Eski resmi al (silme için)
                    var eskiEtkinlik = await _context.Etkinlikler.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    var eskiResimYolu = eskiEtkinlik?.ResimYolu;

                    // Yeni dosya yükleme işlemi
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                            resimDosyasi, 
                            "uploads/etkinlikler",
                            5242880); // 5MB limit

                        if (success && !string.IsNullOrEmpty(fileName))
                        {
                            etkinlik.ResimYolu = "/" + fileName;

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
                            return View(etkinlik);
                        }
                    }
                    else if (string.IsNullOrEmpty(etkinlik.ResimYolu) && !string.IsNullOrEmpty(eskiResimYolu))
                    {
                        // Resim yolu temizlendiyse eski resmi sil
                        if (eskiResimYolu.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiResimYolu.TrimStart('/'));
                        }
                    }
                    else if (!string.IsNullOrEmpty(eskiResimYolu) && etkinlik.ResimYolu != eskiResimYolu && !etkinlik.ResimYolu.StartsWith("/uploads/"))
                    {
                        // Manuel URL girildiyse eski yüklenen dosyayı sil
                        if (eskiResimYolu.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiResimYolu.TrimStart('/'));
                        }
                    }

                    _context.Update(etkinlik);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Etkinlik", $"Etkinlik güncellendi: {etkinlik.Baslik} (ID: {etkinlik.Id})");

                    TempData["Success"] = "Etkinlik başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EtkinlikExists(etkinlik.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(etkinlik);
        }

        /// <summary>
        /// Etkinlik silme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null)
            {
                return NotFound();
            }

            var etkinlikBaslik = etkinlik.Baslik;

            _context.Etkinlikler.Remove(etkinlik);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Etkinlik", $"Etkinlik silindi: {etkinlikBaslik} (ID: {id})");

            TempData["Success"] = "Etkinlik başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Etkinlik var mı kontrolü
        /// </summary>
        private bool EtkinlikExists(int id)
        {
            return _context.Etkinlikler.Any(e => e.Id == id);
        }
    }
}

