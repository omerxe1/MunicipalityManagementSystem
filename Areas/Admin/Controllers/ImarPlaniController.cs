using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İmar planı yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class ImarPlaniController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public ImarPlaniController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var planlar = await _context.ImarPlanlari.OrderByDescending(i => i.OlusturmaTarihi).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(planlar);
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
        public async Task<IActionResult> Create(ImarPlani plan)
        {
            if (ModelState.IsValid)
            {
                plan.OlusturmaTarihi = DateTime.Now;
                _context.ImarPlanlari.Add(plan);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "ImarPlani", $"İmar planı eklendi: {plan.Baslik}");

                TempData["Success"] = "İmar planı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.ImarPlanlari.FindAsync(id);
            if (plan == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ImarPlani plan)
        {
            if (id != plan.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "ImarPlani", $"İmar planı güncellendi: {plan.Baslik} (ID: {plan.Id})");

                    TempData["Success"] = "İmar planı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImarPlaniExists(plan.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.ImarPlanlari.FindAsync(id);
            if (plan == null) return NotFound();
            var planBaslik = plan.Baslik;

            _context.ImarPlanlari.Remove(plan);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "ImarPlani", $"İmar planı silindi: {planBaslik} (ID: {id})");

            TempData["Success"] = "İmar planı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool ImarPlaniExists(int id) => _context.ImarPlanlari.Any(e => e.Id == id);
    }
}

