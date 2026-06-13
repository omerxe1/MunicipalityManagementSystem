using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Models;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    /// <summary>
    /// Bilgi edinme başvurusu yönetimi controller'ı
    /// FAZ 4: Log sistemi entegre edildi
    /// </summary>
    [AdminAuthorize]
    public class BilgiEdinmeBasvuruController : BaseAdminController
    {
        private readonly KocaaliContext _context;
        public BilgiEdinmeBasvuruController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService) => _context = context;

        public async Task<IActionResult> Index()
        {
            var basvurular = await _context.BilgiEdinmeBasvurulari.OrderByDescending(b => b.OlusturmaTarihi).ToListAsync();
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            return View(basvurular);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var basvuru = await _context.BilgiEdinmeBasvurulari.FindAsync(id);
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
            var basvuru = await _context.BilgiEdinmeBasvurulari.FindAsync(id);
            if (basvuru == null) return NotFound();
            var basvuruSahis = basvuru.SahisTuru == "Sahis" 
                ? $"{basvuru.Ad} {basvuru.Soyad}" 
                : basvuru.Unvan ?? "Bilinmeyen";

            _context.BilgiEdinmeBasvurulari.Remove(basvuru);
            await _context.SaveChangesAsync();

            // FAZ 4: Log kaydı
            await LogAsync("Silme", "BilgiEdinmeBasvuru", $"Bilgi edinme başvurusu silindi: {basvuruSahis} (ID: {id})");

            TempData["Success"] = "Bilgi edinme başvurusu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

