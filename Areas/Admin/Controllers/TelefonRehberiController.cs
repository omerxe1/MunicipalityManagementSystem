using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Telefon rehberi yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class TelefonRehberiController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public TelefonRehberiController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var rehber = await _context.TelefonRehberi.OrderBy(t => t.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rehber);
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
        public async Task<IActionResult> Create(TelefonRehberi rehber)
        {
            if (ModelState.IsValid)
            {
                _context.TelefonRehberi.Add(rehber);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "TelefonRehberi", $"Telefon rehberi kaydı eklendi: {rehber.KurumAdi} - {rehber.Telefon}");

                TempData["Success"] = "Telefon rehberi kaydı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rehber);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var rehber = await _context.TelefonRehberi.FindAsync(id);
            if (rehber == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rehber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TelefonRehberi rehber)
        {
            if (id != rehber.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rehber);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "TelefonRehberi", $"Telefon rehberi kaydı güncellendi: {rehber.KurumAdi} - {rehber.Telefon} (ID: {rehber.Id})");

                    TempData["Success"] = "Telefon rehberi kaydı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelefonRehberiExists(rehber.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(rehber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rehber = await _context.TelefonRehberi.FindAsync(id);
            if (rehber == null) return NotFound();
            var rehberBilgi = $"{rehber.KurumAdi} - {rehber.Telefon}";

            _context.TelefonRehberi.Remove(rehber);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "TelefonRehberi", $"Telefon rehberi kaydı silindi: {rehberBilgi} (ID: {id})");

            TempData["Success"] = "Telefon rehberi kaydı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool TelefonRehberiExists(int id) => _context.TelefonRehberi.Any(e => e.Id == id);
    }
}

