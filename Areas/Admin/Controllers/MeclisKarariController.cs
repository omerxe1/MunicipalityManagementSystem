using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Meclis kararı yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class MeclisKarariController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        private readonly IFileUploadService _fileUploadService;

        public MeclisKarariController(KocaaliContext context, IAdminLogService adminLogService, IFileUploadService fileUploadService)
            : base(adminLogService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var kararlar = await _context.MeclisKararlari.OrderByDescending(k => k.Tarih).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kararlar);
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
        public async Task<IActionResult> Create(MeclisKarari karar, IFormFile? pdfDosyasi)
        {
            if (ModelState.IsValid)
            {
                // PDF dosya yükleme işlemi
                if (pdfDosyasi != null && pdfDosyasi.Length > 0)
                {
                    var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                        pdfDosyasi, 
                        "uploads/meclis-kararlari",
                        10485760); // 10MB limit (PDF'ler daha büyük olabilir)

                    if (success && !string.IsNullOrEmpty(fileName))
                    {
                        karar.PdfUrl = "/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("", errorMessage ?? "Dosya yükleme hatası.");
                        ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
                        ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
                        return View(karar);
                    }
                }

                _context.MeclisKararlari.Add(karar);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "MeclisKarari", $"Meclis kararı eklendi: {karar.Baslik}");

                TempData["Success"] = "Meclis kararı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(karar);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var karar = await _context.MeclisKararlari.FindAsync(id);
            if (karar == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(karar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeclisKarari karar, IFormFile? pdfDosyasi)
        {
            if (id != karar.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    // Eski PDF'i al (silme için)
                    var eskiKarar = await _context.MeclisKararlari.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
                    var eskiPdfUrl = eskiKarar?.PdfUrl;

                    // Yeni PDF dosya yükleme işlemi
                    if (pdfDosyasi != null && pdfDosyasi.Length > 0)
                    {
                        var (success, fileName, errorMessage) = await _fileUploadService.UploadFileAsync(
                            pdfDosyasi, 
                            "uploads/meclis-kararlari",
                            10485760); // 10MB limit

                        if (success && !string.IsNullOrEmpty(fileName))
                        {
                            karar.PdfUrl = "/" + fileName;

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
                            return View(karar);
                        }
                    }
                    else if (string.IsNullOrEmpty(karar.PdfUrl) && !string.IsNullOrEmpty(eskiPdfUrl))
                    {
                        // PDF URL temizlendiyse eski PDF'i sil
                        if (eskiPdfUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiPdfUrl.TrimStart('/'));
                        }
                    }
                    else if (!string.IsNullOrEmpty(eskiPdfUrl) && karar.PdfUrl != eskiPdfUrl && !karar.PdfUrl.StartsWith("/uploads/"))
                    {
                        // Manuel URL girildiyse eski yüklenen dosyayı sil
                        if (eskiPdfUrl.StartsWith("/uploads/"))
                        {
                            await _fileUploadService.DeleteFileAsync(eskiPdfUrl.TrimStart('/'));
                        }
                    }

                    _context.Update(karar);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "MeclisKarari", $"Meclis kararı güncellendi: {karar.Baslik} (ID: {karar.Id})");

                    TempData["Success"] = "Meclis kararı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeclisKarariExists(karar.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(karar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var karar = await _context.MeclisKararlari.FindAsync(id);
            if (karar == null) return NotFound();
            var kararBaslik = karar.Baslik;

            _context.MeclisKararlari.Remove(karar);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "MeclisKarari", $"Meclis kararı silindi: {kararBaslik} (ID: {id})");

            TempData["Success"] = "Meclis kararı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool MeclisKarariExists(int id) => _context.MeclisKararlari.Any(e => e.Id == id);
    }
}

