using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Takvim günü yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class TakvimGunuController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public TakvimGunuController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var takvimGunleri = await _context.TakvimGunleri.OrderByDescending(t => t.Tarih).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(takvimGunleri);
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
        public async Task<IActionResult> Create(TakvimGunu takvimGunu)
        {
            if (ModelState.IsValid)
            {
                _context.TakvimGunleri.Add(takvimGunu);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "TakvimGunu", $"Takvim günü eklendi: {takvimGunu.Baslik} ({takvimGunu.Tarih:dd.MM.yyyy})");

                TempData["Success"] = "Takvim günü başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(takvimGunu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var takvimGunu = await _context.TakvimGunleri.FindAsync(id);
            if (takvimGunu == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(takvimGunu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TakvimGunu takvimGunu)
        {
            if (id != takvimGunu.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(takvimGunu);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "TakvimGunu", $"Takvim günü güncellendi: {takvimGunu.Baslik} (ID: {takvimGunu.Id})");

                    TempData["Success"] = "Takvim günü başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TakvimGunuExists(takvimGunu.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(takvimGunu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var takvimGunu = await _context.TakvimGunleri.FindAsync(id);
            if (takvimGunu == null) return NotFound();
            var takvimGunuBaslik = takvimGunu.Baslik;

            _context.TakvimGunleri.Remove(takvimGunu);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "TakvimGunu", $"Takvim günü silindi: {takvimGunuBaslik} (ID: {id})");

            TempData["Success"] = "Takvim günü başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool TakvimGunuExists(int id) => _context.TakvimGunleri.Any(e => e.Id == id);
    }
}

