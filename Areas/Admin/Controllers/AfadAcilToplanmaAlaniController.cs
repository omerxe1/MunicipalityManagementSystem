using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// AFAD acil toplanma alanı yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class AfadAcilToplanmaAlaniController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public AfadAcilToplanmaAlaniController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var alanlar = await _context.AfadAcilToplanmaAlanlari.OrderBy(a => a.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(alanlar);
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
        public async Task<IActionResult> Create(AfadAcilToplanmaAlani alan)
        {
            if (ModelState.IsValid)
            {
                _context.AfadAcilToplanmaAlanlari.Add(alan);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "AfadAcilToplanmaAlani", $"AFAD acil toplanma alanı eklendi: {alan.Adi}");

                TempData["Success"] = "AFAD acil toplanma alanı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(alan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var alan = await _context.AfadAcilToplanmaAlanlari.FindAsync(id);
            if (alan == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(alan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AfadAcilToplanmaAlani alan)
        {
            if (id != alan.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alan);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "AfadAcilToplanmaAlani", $"AFAD acil toplanma alanı güncellendi: {alan.Adi} (ID: {alan.Id})");

                    TempData["Success"] = "AFAD acil toplanma alanı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AfadAcilToplanmaAlaniExists(alan.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(alan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var alan = await _context.AfadAcilToplanmaAlanlari.FindAsync(id);
            if (alan == null) return NotFound();
            var alanAdi = alan.Adi;

            _context.AfadAcilToplanmaAlanlari.Remove(alan);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "AfadAcilToplanmaAlani", $"AFAD acil toplanma alanı silindi: {alanAdi} (ID: {id})");

            TempData["Success"] = "AFAD acil toplanma alanı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool AfadAcilToplanmaAlaniExists(int id) => _context.AfadAcilToplanmaAlanlari.Any(e => e.Id == id);
    }
}

