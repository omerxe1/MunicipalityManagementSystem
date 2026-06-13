using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İletişim formu yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IletisimFormuController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public IletisimFormuController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var formlar = await _context.IletisimFormlari.OrderByDescending(i => i.OlusturmaTarihi).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(formlar);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var form = await _context.IletisimFormlari.FindAsync(id);
            if (form == null) return NotFound();
            
            if (!form.OkunduMu)
            {
                form.OkunduMu = true;
                _context.Update(form);
                await _context.SaveChangesAsync();
            }

            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var form = await _context.IletisimFormlari.FindAsync(id);
            if (form == null) return NotFound();
            var formAdSoyad = $"{form.Ad} {form.Soyad}";

            _context.IletisimFormlari.Remove(form);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "IletisimFormu", $"İletişim formu silindi: {formAdSoyad} (ID: {id})");

            TempData["Success"] = "İletişim formu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

