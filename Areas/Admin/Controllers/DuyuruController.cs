using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Duyuru yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class DuyuruController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public DuyuruController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService)
        {
            _context = context;
        }

        /// <summary>
        /// Duyuru listesi sayfası
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var duyurular = await _context.Duyurular
                .OrderByDescending(d => d.Tarih)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");

            return View(duyurular);
        }

        /// <summary>
        /// Yeni duyuru ekleme sayfası
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View();
        }

        /// <summary>
        /// Yeni duyuru ekleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Duyuru duyuru)
        {
            if (ModelState.IsValid)
            {
                if (duyuru.Tarih == default)
                {
                    duyuru.Tarih = DateTime.Now;
                }

                _context.Duyurular.Add(duyuru);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "Duyuru", $"Duyuru eklendi: {duyuru.Baslik}");

                TempData["Success"] = "Duyuru başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(duyuru);
        }

        /// <summary>
        /// Duyuru düzenleme sayfası
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var duyuru = await _context.Duyurular.FindAsync(id);
            if (duyuru == null)
            {
                return NotFound();
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(duyuru);
        }

        /// <summary>
        /// Duyuru düzenleme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Duyuru duyuru)
        {
            if (id != duyuru.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(duyuru);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "Duyuru", $"Duyuru güncellendi: {duyuru.Baslik} (ID: {duyuru.Id})");

                    TempData["Success"] = "Duyuru başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DuyuruExists(duyuru.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(duyuru);
        }

        /// <summary>
        /// Duyuru silme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var duyuru = await _context.Duyurular.FindAsync(id);
            if (duyuru == null)
            {
                return NotFound();
            }

            var duyuruBaslik = duyuru.Baslik;

            _context.Duyurular.Remove(duyuru);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "Duyuru", $"Duyuru silindi: {duyuruBaslik} (ID: {id})");

            TempData["Success"] = "Duyuru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Duyuru var mı kontrolü
        /// </summary>
        private bool DuyuruExists(int id)
        {
            return _context.Duyurular.Any(e => e.Id == id);
        }
    }
}

