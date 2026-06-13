using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Fotoğraf yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class PhotoController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        private readonly IFileUploadService _fileUploadService;

        public PhotoController(KocaaliContext context, IAdminLogService adminLogService, IFileUploadService fileUploadService)
            : base(adminLogService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var photos = await _context.Photos.OrderByDescending(p => p.UploadDate).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(photos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Photo photo, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                // Dosya yükleme işlemi
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                        resimDosyasi, 
                        "uploads/fotograflar",
                        5242880); // 5MB limit

                    if (success && !string.IsNullOrEmpty(fileName))
                    {
                        photo.ImageUrl = "/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(photo);
                    }
                }
                else if (string.IsNullOrEmpty(photo.ImageUrl))
                {
                    ModelState.AddModelError("ImageUrl", "Resim dosyası veya URL girilmelidir.");
                    ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                    ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                    return View(photo);
                }

                photo.UploadDate = DateTime.Now;
                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Photo", $"Fotoğraf eklendi: {photo.Title}");

                TempData["Success"] = "Fotoğraf başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(photo);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Photo photo, IFormFile? resimDosyasi)
        {
            if (id != photo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    // Eski resmi al (silme için)
                    var eskiPhoto = await _context.Photos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    var eskiImageUrl = eskiPhoto?.ImageUrl;

                    // Yeni dosya yükleme işlemi
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                            resimDosyasi, 
                            "uploads/fotograflar",
                            5242880); // 5MB limit

                        if (success && !string.IsNullOrEmpty(fileName))
                        {
                            photo.ImageUrl = "/" + fileName;

                            // Eski resmi sil
                            if (!string.IsNullOrEmpty(eskiImageUrl) && eskiImageUrl.StartsWith("/uploads/"))
                            {
                                await _fileUploadService.DeleteFileAsync(eskiImageUrl.TrimStart('/'));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                            return View(photo);
                        }
                    }
                    else if (string.IsNullOrEmpty(photo.ImageUrl) && !string.IsNullOrEmpty(eskiImageUrl))
                    {
                        // Resim yolu temizlendiyse eski resmi sil
                        if (eskiImageUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiImageUrl.TrimStart('/'));
                        }
                    }
                    else if (!string.IsNullOrEmpty(eskiImageUrl) && photo.ImageUrl != eskiImageUrl && !photo.ImageUrl.StartsWith("/uploads/"))
                    {
                        // Manuel URL girildiyse eski yüklenen dosyayı sil
                        if (eskiImageUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiImageUrl.TrimStart('/'));
                        }
                    }

                    _context.Update(photo);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Photo", $"Fotoğraf güncellendi: {photo.Title} (ID: {photo.Id})");

                    TempData["Success"] = "Fotoğraf başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(photo.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            var photoTitle = photo.Title;

            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Photo", $"Fotoğraf silindi: {photoTitle} (ID: {id})");

            TempData["Success"] = "Fotoğraf başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id) => _context.Photos.Any(e => e.Id == id);
    }
}

