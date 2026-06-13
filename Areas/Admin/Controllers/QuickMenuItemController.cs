using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Hızlı menü öğesi yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class QuickMenuItemController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public QuickMenuItemController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var menuler = await _context.QuickMenuItems.OrderBy(q => q.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(menuler);
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
        public async Task<IActionResult> Create(QuickMenuItem menu)
        {
            if (ModelState.IsValid)
            {
                _context.QuickMenuItems.Add(menu);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "QuickMenuItem", $"Hızlı menü öğesi eklendi: {menu.Baslik}");

                TempData["Success"] = "Hızlı menü öğesi başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(menu);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var menu = await _context.QuickMenuItems.FindAsync(id);
            if (menu == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuickMenuItem menu)
        {
            if (id != menu.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "QuickMenuItem", $"Hızlı menü öğesi güncellendi: {menu.Baslik} (ID: {menu.Id})");

                    TempData["Success"] = "Hızlı menü öğesi başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuickMenuItemExists(menu.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.QuickMenuItems.FindAsync(id);
            if (menu == null) return NotFound();
            var menuBaslik = menu.Baslik;

            _context.QuickMenuItems.Remove(menu);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "QuickMenuItem", $"Hızlı menü öğesi silindi: {menuBaslik} (ID: {id})");

            TempData["Success"] = "Hızlı menü öğesi başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool QuickMenuItemExists(int id) => _context.QuickMenuItems.Any(e => e.Id == id);
    }
}

