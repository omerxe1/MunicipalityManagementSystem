using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İhale yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IhaleController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public IhaleController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var ihaleler = await _context.Ihaleler.OrderByDescending(i => i.Tarih).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(ihaleler);
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
        public async Task<IActionResult> Create(Ihale ihale)
        {
            if (ModelState.IsValid)
            {
                ihale.OlusturmaTarihi = DateTime.Now;
                _context.Ihaleler.Add(ihale);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Ihale", $"İhale eklendi: {ihale.Baslik}");

                TempData["Success"] = "İhale başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(ihale);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ihale = await _context.Ihaleler.FindAsync(id);
            if (ihale == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(ihale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ihale ihale)
        {
            if (id != ihale.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ihale);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Ihale", $"İhale güncellendi: {ihale.Baslik} (ID: {ihale.Id})");

                    TempData["Success"] = "İhale başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IhaleExists(ihale.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(ihale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);
            if (ihale == null) return NotFound();
            var ihaleBaslik = ihale.Baslik;

            _context.Ihaleler.Remove(ihale);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Ihale", $"İhale silindi: {ihaleBaslik} (ID: {id})");

            TempData["Success"] = "İhale başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool IhaleExists(int id) => _context.Ihaleler.Any(e => e.Id == id);
    }
}

