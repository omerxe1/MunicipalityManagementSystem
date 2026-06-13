using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Kardeş şehir yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class KardesSehirController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public KardesSehirController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var sehirler = await _context.KardesSehirler.OrderBy(k => k.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sehirler);
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
        public async Task<IActionResult> Create(KardesSehir sehir)
        {
            if (ModelState.IsValid)
            {
                _context.KardesSehirler.Add(sehir);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "KardesSehir", $"Kardeş şehir eklendi: {sehir.BelediyeAdi} - {sehir.SehirAdi}");

                TempData["Success"] = "Kardeş şehir başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sehir);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sehir = await _context.KardesSehirler.FindAsync(id);
            if (sehir == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sehir);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KardesSehir sehir)
        {
            if (id != sehir.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sehir);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "KardesSehir", $"Kardeş şehir güncellendi: {sehir.BelediyeAdi} - {sehir.SehirAdi} (ID: {sehir.Id})");

                    TempData["Success"] = "Kardeş şehir başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KardesSehirExists(sehir.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sehir);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sehir = await _context.KardesSehirler.FindAsync(id);
            if (sehir == null) return NotFound();
            var sehirBilgi = $"{sehir.BelediyeAdi} - {sehir.SehirAdi}";

            _context.KardesSehirler.Remove(sehir);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "KardesSehir", $"Kardeş şehir silindi: {sehirBilgi} (ID: {id})");

            TempData["Success"] = "Kardeş şehir başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool KardesSehirExists(int id) => _context.KardesSehirler.Any(e => e.Id == id);
    }
}

