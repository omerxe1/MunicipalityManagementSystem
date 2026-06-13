using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Performans programı yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class PerformansProgramiController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public PerformansProgramiController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var programlar = await _context.PerformansProgramlari.OrderByDescending(p => p.MaliYil).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(programlar);
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
        public async Task<IActionResult> Create(PerformansProgrami program)
        {
            if (ModelState.IsValid)
            {
                program.OlusturmaTarihi = DateTime.Now;
                _context.PerformansProgramlari.Add(program);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "PerformansProgrami", $"Performans programı eklendi: Mali Yıl {program.MaliYil}");

                TempData["Success"] = "Performans programı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(program);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var program = await _context.PerformansProgramlari.FindAsync(id);
            if (program == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PerformansProgrami program)
        {
            if (id != program.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(program);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "PerformansProgrami", $"Performans programı güncellendi: Mali Yıl {program.MaliYil} (ID: {program.Id})");

                    TempData["Success"] = "Performans programı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerformansProgramiExists(program.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var program = await _context.PerformansProgramlari.FindAsync(id);
            if (program == null) return NotFound();
            var programBilgi = $"Mali Yıl {program.MaliYil}";

            _context.PerformansProgramlari.Remove(program);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "PerformansProgrami", $"Performans programı silindi: {programBilgi} (ID: {id})");

            TempData["Success"] = "Performans programı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool PerformansProgramiExists(int id) => _context.PerformansProgramlari.Any(e => e.Id == id);
    }
}

