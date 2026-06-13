using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Sıkça Sorulan Sorular yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class SikcaSorulanSoruController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public SikcaSorulanSoruController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var sorular = await _context.SikcaSorulanSorular.OrderBy(s => s.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(sorular);
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
        public async Task<IActionResult> Create(SikcaSorulanSoru soru)
        {
            if (ModelState.IsValid)
            {
                soru.OlusturmaTarihi = DateTime.Now;
                _context.SikcaSorulanSorular.Add(soru);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "SikcaSorulanSoru", $"Soru eklendi: {soru.SoruBaslik}");

                TempData["Success"] = "Soru başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(soru);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var soru = await _context.SikcaSorulanSorular.FindAsync(id);
            if (soru == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(soru);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SikcaSorulanSoru soru)
        {
            if (id != soru.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    soru.GuncellemeTarihi = DateTime.Now;
                    _context.Update(soru);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "SikcaSorulanSoru", $"Soru güncellendi: {soru.SoruBaslik} (ID: {soru.Id})");

                    TempData["Success"] = "Soru başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoruExists(soru.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(soru);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var soru = await _context.SikcaSorulanSorular.FindAsync(id);
            if (soru == null) return NotFound();
            var soruBaslik = soru.SoruBaslik;

            _context.SikcaSorulanSorular.Remove(soru);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "SikcaSorulanSoru", $"Soru silindi: {soruBaslik} (ID: {id})");

            TempData["Success"] = "Soru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool SoruExists(int id) => _context.SikcaSorulanSorular.Any(e => e.Id == id);
    }
}



