using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Kalite yönetim sistemi yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class KaliteYonetimSistemiController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public KaliteYonetimSistemiController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var sistemler = await _context.KaliteYonetimSistemleri.OrderBy(k => k.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sistemler);
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
        public async Task<IActionResult> Create(KaliteYonetimSistemi sistem)
        {
            if (ModelState.IsValid)
            {
                sistem.OlusturmaTarihi = DateTime.Now;
                _context.KaliteYonetimSistemleri.Add(sistem);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "KaliteYonetimSistemi", $"Kalite yönetim sistemi eklendi: {sistem.Baslik}");

                TempData["Success"] = "Kalite yönetim sistemi başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sistem);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sistem = await _context.KaliteYonetimSistemleri.FindAsync(id);
            if (sistem == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sistem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KaliteYonetimSistemi sistem)
        {
            if (id != sistem.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sistem);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "KaliteYonetimSistemi", $"Kalite yönetim sistemi güncellendi: {sistem.Baslik} (ID: {sistem.Id})");

                    TempData["Success"] = "Kalite yönetim sistemi başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KaliteYonetimSistemiExists(sistem.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sistem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sistem = await _context.KaliteYonetimSistemleri.FindAsync(id);
            if (sistem == null) return NotFound();
            var sistemBaslik = sistem.Baslik;

            _context.KaliteYonetimSistemleri.Remove(sistem);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "KaliteYonetimSistemi", $"Kalite yönetim sistemi silindi: {sistemBaslik} (ID: {id})");

            TempData["Success"] = "Kalite yönetim sistemi başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool KaliteYonetimSistemiExists(int id) => _context.KaliteYonetimSistemleri.Any(e => e.Id == id);
    }
}

