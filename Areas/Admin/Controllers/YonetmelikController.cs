using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Yönetmelik yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class YonetmelikController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        private readonly IFileUploadService _fileUploadService;

        public YonetmelikController(KocaaliContext context, IAdminLogService adminLogService, IFileUploadService fileUploadService)
            : base(adminLogService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var yonetmelikler = await _context.Yonetmelikler.OrderBy(y => y.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(yonetmelikler);
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
        public async Task<IActionResult> Create(Yonetmelik yonetmelik, IFormFile? pdfDosyasi)
        {
            if (ModelState.IsValid)
            {
                // PDF dosya yükleme işlemi
                if (pdfDosyasi != null && pdfDosyasi.Length > 0)
                {
                    var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                        pdfDosyasi, 
                        "uploads/yonetmelikler",
                        10485760); // 10MB limit (PDF'ler daha büyük olabilir)

                    if (success && !string.IsNullOrEmpty(fileName))
                    {
                        yonetmelik.PdfUrl = "/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(yonetmelik);
                    }
                }
                else if (string.IsNullOrEmpty(yonetmelik.PdfUrl))
                {
                    ModelState.AddModelError("PdfUrl", "PDF dosyası veya URL girilmelidir.");
                    ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                    ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                    return View(yonetmelik);
                }

                _context.Yonetmelikler.Add(yonetmelik);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Yonetmelik", $"Yönetmelik eklendi: {yonetmelik.Baslik}");

                TempData["Success"] = "Yönetmelik başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(yonetmelik);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var yonetmelik = await _context.Yonetmelikler.FindAsync(id);
            if (yonetmelik == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(yonetmelik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Yonetmelik yonetmelik, IFormFile? pdfDosyasi)
        {
            if (id != yonetmelik.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    // Eski PDF'i al (silme için)
                    var eskiYonetmelik = await _context.Yonetmelikler.AsNoTracking().FirstOrDefaultAsync(y => y.Id == id);
                    var eskiPdfUrl = eskiYonetmelik?.PdfUrl;

                    // Yeni PDF dosya yükleme işlemi
                    if (pdfDosyasi != null && pdfDosyasi.Length > 0)
                    {
                        var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                            pdfDosyasi, 
                            "uploads/yonetmelikler",
                            10485760); // 10MB limit

                        if (success && !string.IsNullOrEmpty(fileName))
                        {
                            yonetmelik.PdfUrl = "/" + fileName;

                            // Eski PDF'i sil
                            if (!string.IsNullOrEmpty(eskiPdfUrl) && eskiPdfUrl.StartsWith("/uploads/"))
                            {
                                await _fileUploadService.DeleteFileAsync(eskiPdfUrl.TrimStart('/'));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                            return View(yonetmelik);
                        }
                    }
                    else if (string.IsNullOrEmpty(yonetmelik.PdfUrl) && !string.IsNullOrEmpty(eskiPdfUrl))
                    {
                        // PDF URL temizlendiyse eski PDF'i sil
                        if (eskiPdfUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiPdfUrl.TrimStart('/'));
                        }
                    }
                    else if (!string.IsNullOrEmpty(eskiPdfUrl) && yonetmelik.PdfUrl != eskiPdfUrl && !yonetmelik.PdfUrl.StartsWith("/uploads/"))
                    {
                        // Manuel URL girildiyse eski yüklenen dosyayı sil
                        if (eskiPdfUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiPdfUrl.TrimStart('/'));
                        }
                    }

                    _context.Update(yonetmelik);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Yonetmelik", $"Yönetmelik güncellendi: {yonetmelik.Baslik} (ID: {yonetmelik.Id})");

                    TempData["Success"] = "Yönetmelik başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YonetmelikExists(yonetmelik.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(yonetmelik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var yonetmelik = await _context.Yonetmelikler.FindAsync(id);
            if (yonetmelik == null) return NotFound();
            var yonetmelikBaslik = yonetmelik.Baslik;

            _context.Yonetmelikler.Remove(yonetmelik);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Yonetmelik", $"Yönetmelik silindi: {yonetmelikBaslik} (ID: {id})");

            TempData["Success"] = "Yönetmelik başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool YonetmelikExists(int id) => _context.Yonetmelikler.Any(e => e.Id == id);
    }
}

