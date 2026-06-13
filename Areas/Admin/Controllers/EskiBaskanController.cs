using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Eski başkan yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class EskiBaskanController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public EskiBaskanController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var eskiBaskanlar = await _context.EskiBaskanlar.OrderBy(e => e.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(eskiBaskanlar);
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
        public async Task<IActionResult> Create(EskiBaskan eskiBaskan)
        {
            if (ModelState.IsValid)
            {
                _context.EskiBaskanlar.Add(eskiBaskan);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "EskiBaskan", $"Eski başkan eklendi: {eskiBaskan.AdSoyad}");

                TempData["Success"] = "Eski başkan başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(eskiBaskan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var eskiBaskan = await _context.EskiBaskanlar.FindAsync(id);
            if (eskiBaskan == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(eskiBaskan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EskiBaskan eskiBaskan)
        {
            if (id != eskiBaskan.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eskiBaskan);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "EskiBaskan", $"Eski başkan güncellendi: {eskiBaskan.AdSoyad} (ID: {eskiBaskan.Id})");

                    TempData["Success"] = "Eski başkan başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EskiBaskanExists(eskiBaskan.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(eskiBaskan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eskiBaskan = await _context.EskiBaskanlar.FindAsync(id);
            if (eskiBaskan == null) return NotFound();
            var eskiBaskanAdSoyad = eskiBaskan.AdSoyad;

            _context.EskiBaskanlar.Remove(eskiBaskan);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "EskiBaskan", $"Eski başkan silindi: {eskiBaskanAdSoyad} (ID: {id})");

            TempData["Success"] = "Eski başkan başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool EskiBaskanExists(int id) => _context.EskiBaskanlar.Any(e => e.Id == id);
    }
}

