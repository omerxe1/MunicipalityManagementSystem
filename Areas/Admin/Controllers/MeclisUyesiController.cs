using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Meclis üyesi yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class MeclisUyesiController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public MeclisUyesiController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var meclisUyeleri = await _context.MeclisUyeleri.OrderBy(m => m.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(meclisUyeleri);
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
        public async Task<IActionResult> Create(MeclisUyesi meclisUyesi)
        {
            if (ModelState.IsValid)
            {
                _context.MeclisUyeleri.Add(meclisUyesi);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "MeclisUyesi", $"Meclis üyesi eklendi: {meclisUyesi.AdSoyad}");

                TempData["Success"] = "Meclis üyesi başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(meclisUyesi);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var meclisUyesi = await _context.MeclisUyeleri.FindAsync(id);
            if (meclisUyesi == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(meclisUyesi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeclisUyesi meclisUyesi)
        {
            if (id != meclisUyesi.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meclisUyesi);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "MeclisUyesi", $"Meclis üyesi güncellendi: {meclisUyesi.AdSoyad} (ID: {meclisUyesi.Id})");

                    TempData["Success"] = "Meclis üyesi başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeclisUyesiExists(meclisUyesi.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(meclisUyesi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var meclisUyesi = await _context.MeclisUyeleri.FindAsync(id);
            if (meclisUyesi == null) return NotFound();
            var meclisUyesiAdSoyad = meclisUyesi.AdSoyad;

            _context.MeclisUyeleri.Remove(meclisUyesi);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "MeclisUyesi", $"Meclis üyesi silindi: {meclisUyesiAdSoyad} (ID: {id})");

            TempData["Success"] = "Meclis üyesi başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool MeclisUyesiExists(int id) => _context.MeclisUyeleri.Any(e => e.Id == id);
    }
}

