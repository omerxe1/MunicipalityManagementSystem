using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Popup duyuru yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class PopupAnnouncementController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public PopupAnnouncementController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var popuplar = await _context.PopupAnnouncements.OrderBy(p => p.SiraNo).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(popuplar);
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
        public async Task<IActionResult> Create(PopupAnnouncement popup)
        {
            if (ModelState.IsValid)
            {
                popup.OlusturmaTarihi = DateTime.Now;
                _context.PopupAnnouncements.Add(popup);
                await _context.SaveChangesAsync();

                // FAZ 4: Log kaydı
                await LogAsync("Ekleme", "PopupAnnouncement", $"Popup duyurusu eklendi: {popup.Title}");

                TempData["Success"] = "Popup duyurusu başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(popup);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var popup = await _context.PopupAnnouncements.FindAsync(id);
            if (popup == null) return NotFound();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(popup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PopupAnnouncement popup)
        {
            if (id != popup.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(popup);
                    await _context.SaveChangesAsync();

                    // FAZ 4: Log kaydı
                    await LogAsync("Güncelleme", "PopupAnnouncement", $"Popup duyurusu güncellendi: {popup.Title} (ID: {popup.Id})");

                    TempData["Success"] = "Popup duyurusu başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PopupAnnouncementExists(popup.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(popup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var popup = await _context.PopupAnnouncements.FindAsync(id);
            if (popup == null) return NotFound();
            var popupBaslik = popup.Title;

            _context.PopupAnnouncements.Remove(popup);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "PopupAnnouncement", $"Popup duyurusu silindi: {popupBaslik} (ID: {id})");

            TempData["Success"] = "Popup duyurusu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool PopupAnnouncementExists(int id) => _context.PopupAnnouncements.Any(e => e.Id == id);
    }
}

