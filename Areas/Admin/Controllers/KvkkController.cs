using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// KVKK döküman yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class KvkkController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public KvkkController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var dokumanlar = await _context.KvkkDokumanlari.OrderBy(k => k.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(dokumanlar);
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
        public async Task<IActionResult> Create(Kvkk kvkk)
        {
            if (ModelState.IsValid)
            {
                kvkk.OlusturmaTarihi = DateTime.Now;
                _context.KvkkDokumanlari.Add(kvkk);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Kvkk", $"KVKK dökümanı eklendi: {kvkk.Baslik}");

                TempData["Success"] = "KVKK dökümanı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kvkk);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var kvkk = await _context.KvkkDokumanlari.FindAsync(id);
            if (kvkk == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kvkk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Kvkk kvkk)
        {
            if (id != kvkk.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kvkk);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Kvkk", $"KVKK dökümanı güncellendi: {kvkk.Baslik} (ID: {kvkk.Id})");

                    TempData["Success"] = "KVKK dökümanı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KvkkExists(kvkk.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(kvkk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var kvkk = await _context.KvkkDokumanlari.FindAsync(id);
            if (kvkk == null) return NotFound();
            var kvkkBaslik = kvkk.Baslik;

            _context.KvkkDokumanlari.Remove(kvkk);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Kvkk", $"KVKK dökümanı silindi: {kvkkBaslik} (ID: {id})");

            TempData["Success"] = "KVKK dökümanı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool KvkkExists(int id) => _context.KvkkDokumanlari.Any(e => e.Id == id);
    }
}

