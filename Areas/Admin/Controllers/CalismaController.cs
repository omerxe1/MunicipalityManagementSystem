using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Çalışma yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class CalismaController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public CalismaController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var calismalar = await _context.Calismalar.OrderByDescending(c => c.PublishDate).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(calismalar);
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
        public async Task<IActionResult> Create(Calisma calisma)
        {
            if (ModelState.IsValid)
            {
                if (calisma.PublishDate == default) calisma.PublishDate = DateTime.Now;
                _context.Calismalar.Add(calisma);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Calisma", $"Çalışma eklendi: {calisma.Title}");

                TempData["Success"] = "Çalışma başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(calisma);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var calisma = await _context.Calismalar.FindAsync(id);
            if (calisma == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(calisma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Calisma calisma)
        {
            if (id != calisma.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calisma);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Calisma", $"Çalışma güncellendi: {calisma.Title} (ID: {calisma.Id})");

                    TempData["Success"] = "Çalışma başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalismaExists(calisma.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(calisma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var calisma = await _context.Calismalar.FindAsync(id);
            if (calisma == null) return NotFound();
            var calismaTitle = calisma.Title;

            _context.Calismalar.Remove(calisma);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Calisma", $"Çalışma silindi: {calismaTitle} (ID: {id})");

            TempData["Success"] = "Çalışma başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool CalismaExists(int id) => _context.Calismalar.Any(e => e.Id == id);
    }
}

