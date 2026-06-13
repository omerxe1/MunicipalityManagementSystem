using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Başvuru kılavuzu yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class BasvuruKilavuzuController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public BasvuruKilavuzuController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var kilavuzlar = await _context.BasvuruKilavuzlari.OrderBy(b => b.Sira).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kilavuzlar);
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
        public async Task<IActionResult> Create(BasvuruKilavuzu kilavuz)
        {
            if (ModelState.IsValid)
            {
                kilavuz.OlusturmaTarihi = DateTime.Now;
                _context.BasvuruKilavuzlari.Add(kilavuz);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "BasvuruKilavuzu", $"Başvuru kılavuzu eklendi: {kilavuz.Baslik}");

                TempData["Success"] = "Başvuru kılavuzu başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kilavuz);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var kilavuz = await _context.BasvuruKilavuzlari.FindAsync(id);
            if (kilavuz == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kilavuz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BasvuruKilavuzu kilavuz)
        {
            if (id != kilavuz.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kilavuz);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "BasvuruKilavuzu", $"Başvuru kılavuzu güncellendi: {kilavuz.Baslik} (ID: {kilavuz.Id})");

                    TempData["Success"] = "Başvuru kılavuzu başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasvuruKilavuzuExists(kilavuz.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kilavuz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kilavuz = await _context.BasvuruKilavuzlari.FindAsync(id);
            if (kilavuz == null) return NotFound();
            var kilavuzBaslik = kilavuz.Baslik;

            _context.BasvuruKilavuzlari.Remove(kilavuz);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "BasvuruKilavuzu", $"Başvuru kılavuzu silindi: {kilavuzBaslik} (ID: {id})");

            TempData["Success"] = "Başvuru kılavuzu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool BasvuruKilavuzuExists(int id) => _context.BasvuruKilavuzlari.Any(e => e.Id == id);
    }
}

