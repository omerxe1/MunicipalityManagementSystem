using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İç kontrol uyum eylem planı yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IcKontrolUyumEylemPlaniController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public IcKontrolUyumEylemPlaniController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var planlar = await _context.IcKontrolUyumEylemPlanlari.OrderBy(i => i.SiraNo).ToListAsync();
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
        public async Task<IActionResult> Create(IcKontrolUyumEylemPlani plan)
        {
            if (ModelState.IsValid)
            {
                plan.OlusturmaTarihi = DateTime.Now;
                _context.IcKontrolUyumEylemPlanlari.Add(plan);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "IcKontrolUyumEylemPlani", $"İç kontrol uyum eylem planı eklendi: {plan.Baslik}");

                TempData["Success"] = "İç kontrol uyum eylem planı başarıyla eklendi.";
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
            var plan = await _context.IcKontrolUyumEylemPlanlari.FindAsync(id);
            if (plan == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IcKontrolUyumEylemPlani plan)
        {
            if (id != plan.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "IcKontrolUyumEylemPlani", $"İç kontrol uyum eylem planı güncellendi: {plan.Baslik} (ID: {plan.Id})");

                    TempData["Success"] = "İç kontrol uyum eylem planı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IcKontrolUyumEylemPlaniExists(plan.Id)) return NotFound();
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
            var plan = await _context.IcKontrolUyumEylemPlanlari.FindAsync(id);
            if (plan == null) return NotFound();
            var planBaslik = plan.Baslik;

            _context.IcKontrolUyumEylemPlanlari.Remove(plan);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "IcKontrolUyumEylemPlani", $"İç kontrol uyum eylem planı silindi: {planBaslik} (ID: {id})");

            TempData["Success"] = "İç kontrol uyum eylem planı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool IcKontrolUyumEylemPlaniExists(int id) => _context.IcKontrolUyumEylemPlanlari.Any(e => e.Id == id);
    }
}

