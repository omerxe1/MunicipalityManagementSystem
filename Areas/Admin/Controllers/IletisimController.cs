using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İletişim başvuruları yönetimi controller'ı
    /// Sadece Admin rolü erişebilir
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IletisimController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public IletisimController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService)
        {
            _context = context;
        }

        /// <summary>
        /// İletişim başvuruları listesi sayfası
        /// Sadece Admin rolü erişebilir
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Rol kontrolü - Sadece Admin erişebilir
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            var basvurular = await _context.IletisimBasvurulari
                .OrderByDescending(b => b.Tarih)
                .ToListAsync();

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");

            return View(basvurular);
        }

        /// <summary>
        /// İletişim başvurusu detay sayfası
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Rol kontrolü - Sadece Admin erişebilir
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (id == null)
            {
                return NotFound();
            }

            var basvuru = await _context.IletisimBasvurulari.FindAsync(id);
            if (basvuru == null)
            {
                return NotFound();
            }

            // Okundu olarak işaretle
            if (!basvuru.OkunduMu)
            {
                basvuru.OkunduMu = true;
                _context.Update(basvuru);
                await _context.SaveChangesAsync();
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");

            return View(basvuru);
        }

        /// <summary>
        /// İletişim başvurusu silme işlemi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Rol kontrolü - Sadece Admin erişebilir
            var rol = HttpContext.Session.GetString("AdminRol");
            if (rol != "Admin")
            {
                TempData["Error"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Dashboard");
            }

            var basvuru = await _context.IletisimBasvurulari.FindAsync(id);
            if (basvuru == null)
            {
                return NotFound();
            }

            var basvuruKonu = basvuru.Konu;

            _context.IletisimBasvurulari.Remove(basvuru);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "IletisimBasvuru", $"İletişim başvurusu silindi: {basvuruKonu} (ID: {id})");

            TempData["Success"] = "Başvuru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

