using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// İstek ve öneri yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class IstekVeOneriController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public IstekVeOneriController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var basvurular = await _context.IstekVeOneriler.OrderByDescending(i => i.OlusturmaTarihi).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(basvurular);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var basvuru = await _context.IstekVeOneriler.FindAsync(id);
            if (basvuru == null) return NotFound();
            
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var basvuru = await _context.IstekVeOneriler.FindAsync(id);
            if (basvuru == null) return NotFound();
            var basvuruAdSoyad = $"{basvuru.Ad} {basvuru.Soyad}";

            _context.IstekVeOneriler.Remove(basvuru);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "IstekVeOneri", $"İstek ve öneri silindi: {basvuruAdSoyad} (ID: {id})");

            TempData["Success"] = "İstek ve öneri başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

