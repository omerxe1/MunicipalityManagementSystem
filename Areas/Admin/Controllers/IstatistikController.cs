using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İstatistik yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IstatistikController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public IstatistikController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var istatistikler = await _context.Istatistikler.OrderBy(i => i.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(istatistikler);
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
        public async Task<IActionResult> Create(Istatistik istatistik)
        {
            if (ModelState.IsValid)
            {
                istatistik.OlusturmaTarihi = DateTime.Now;
                _context.Istatistikler.Add(istatistik);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Istatistik", $"İstatistik eklendi: {istatistik.Baslik}");

                TempData["Success"] = "İstatistik başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(istatistik);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var istatistik = await _context.Istatistikler.FindAsync(id);
            if (istatistik == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(istatistik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Istatistik istatistik)
        {
            if (id != istatistik.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(istatistik);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Istatistik", $"İstatistik güncellendi: {istatistik.Baslik} (ID: {istatistik.Id})");

                    TempData["Success"] = "İstatistik başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IstatistikExists(istatistik.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(istatistik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var istatistik = await _context.Istatistikler.FindAsync(id);
            if (istatistik == null) return NotFound();
            var istatistikBaslik = istatistik.Baslik;

            _context.Istatistikler.Remove(istatistik);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Istatistik", $"İstatistik silindi: {istatistikBaslik} (ID: {id})");

            TempData["Success"] = "İstatistik başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool IstatistikExists(int id) => _context.Istatistikler.Any(e => e.Id == id);
    }
}



