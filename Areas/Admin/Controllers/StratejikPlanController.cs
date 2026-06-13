using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Stratejik plan yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class StratejikPlanController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public StratejikPlanController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var planlar = await _context.StratejikPlanlar.OrderBy(s => s.SiraNo).ToListAsync();
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
        public async Task<IActionResult> Create(StratejikPlan plan)
        {
            if (ModelState.IsValid)
            {
                plan.OlusturmaTarihi = DateTime.Now;
                _context.StratejikPlanlar.Add(plan);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "StratejikPlan", $"Stratejik plan eklendi: {plan.Baslik}");

                TempData["Success"] = "Stratejik plan başarıyla eklendi.";
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
            var plan = await _context.StratejikPlanlar.FindAsync(id);
            if (plan == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StratejikPlan plan)
        {
            if (id != plan.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "StratejikPlan", $"Stratejik plan güncellendi: {plan.Baslik} (ID: {plan.Id})");

                    TempData["Success"] = "Stratejik plan başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StratejikPlanExists(plan.Id)) return NotFound();
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
            var plan = await _context.StratejikPlanlar.FindAsync(id);
            if (plan == null) return NotFound();
            var planBaslik = plan.Baslik;

            _context.StratejikPlanlar.Remove(plan);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "StratejikPlan", $"Stratejik plan silindi: {planBaslik} (ID: {id})");

            TempData["Success"] = "Stratejik plan başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool StratejikPlanExists(int id) => _context.StratejikPlanlar.Any(e => e.Id == id);
    }
}

