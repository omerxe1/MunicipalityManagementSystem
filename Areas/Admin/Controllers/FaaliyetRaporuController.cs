using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Faaliyet raporu yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class FaaliyetRaporuController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public FaaliyetRaporuController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var raporlar = await _context.FaaliyetRaporlari.OrderByDescending(f => f.Yil).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(raporlar);
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
        public async Task<IActionResult> Create(FaaliyetRaporu rapor)
        {
            if (ModelState.IsValid)
            {
                rapor.OlusturmaTarihi = DateTime.Now;
                _context.FaaliyetRaporlari.Add(rapor);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "FaaliyetRaporu", $"Faaliyet raporu eklendi: Yıl {rapor.Yil}");

                TempData["Success"] = "Faaliyet raporu başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rapor);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var rapor = await _context.FaaliyetRaporlari.FindAsync(id);
            if (rapor == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rapor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaaliyetRaporu rapor)
        {
            if (id != rapor.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rapor);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "FaaliyetRaporu", $"Faaliyet raporu güncellendi: Yıl {rapor.Yil} (ID: {rapor.Id})");

                    TempData["Success"] = "Faaliyet raporu başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaaliyetRaporuExists(rapor.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rapor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rapor = await _context.FaaliyetRaporlari.FindAsync(id);
            if (rapor == null) return NotFound();
            var raporBilgi = $"Yıl {rapor.Yil}";

            _context.FaaliyetRaporlari.Remove(rapor);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "FaaliyetRaporu", $"Faaliyet raporu silindi: {raporBilgi} (ID: {id})");

            TempData["Success"] = "Faaliyet raporu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool FaaliyetRaporuExists(int id) => _context.FaaliyetRaporlari.Any(e => e.Id == id);
    }
}

