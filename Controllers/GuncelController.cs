using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Controllers
{
    public class GuncelController : Controller
    {
        private readonly KocaaliContext _context;

        public GuncelController(KocaaliContext context)
        {
            _context = context;
        }

        public IActionResult NobetciEczaneler()
        {
            return View();
        }

        public async Task<IActionResult> AfadAcilToplanmaAlanlari()
        {
            var toplanmaAlanlari = await _context.AfadAcilToplanmaAlanlari
                .Where(x => x.AktifMi)
                .OrderBy(x => x.SiraNo)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return View(toplanmaAlanlari);
        }

        public async Task<IActionResult> Etkinlikler()
        {
            var etkinlikler = await _context.Etkinlikler
                .OrderByDescending(e => e.Tarih)
                .ToListAsync();

            var sonEklenenEtkinlikler = await _context.Etkinlikler
                .OrderByDescending(e => e.Tarih)
                .Take(6)
                .ToListAsync();

            ViewBag.SonEklenenEtkinlikler = sonEklenenEtkinlikler;

            return View(etkinlikler);
        }

        public async Task<IActionResult> Duyurular(int page = 1)
        {
            int pageSize = 10;
            int skip = (page - 1) * pageSize;

            var totalDuyurular = await _context.Announcements.CountAsync();
            var duyurular = await _context.Announcements
                .OrderByDescending(d => d.PublishDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            ViewData["PageTitle"] = "Duyurular";
            ViewData["ContentTitle"] = "DUYURULAR";
            ViewBag.ShowBackButton = true;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalDuyurular / (double)pageSize);
            ViewBag.TotalDuyurular = totalDuyurular;

            return View(duyurular);
        }

        public async Task<IActionResult> DuyuruDetay(int id)
        {
            var duyuru = await _context.Announcements.FindAsync(id);

            if (duyuru == null)
            {
                return NotFound();
            }

            ViewData["PageTitle"] = "Duyuru Detay";
            ViewData["ContentTitle"] = duyuru.Title;
            ViewBag.ShowBackButton = true;

            return View(duyuru);
        }

        public async Task<IActionResult> Ihaleler(string? durum = null)
        {
            var query = _context.Ihaleler.AsQueryable();

            if (!string.IsNullOrEmpty(durum))
            {
                query = query.Where(i => i.Durum == durum);
            }

            var ihaleler = await query
                .OrderByDescending(i => i.Tarih)
                .ToListAsync();

            ViewBag.SeciliDurum = durum;
            ViewBag.DevamEdenSayisi = await _context.Ihaleler.CountAsync(i => i.Durum == "Devam Eden");
            ViewBag.TamamlananSayisi = await _context.Ihaleler.CountAsync(i => i.Durum == "Tamamlanan");
            ViewBag.PlanlananSayisi = await _context.Ihaleler.CountAsync(i => i.Durum == "Planlanan");

            return View(ihaleler);
        }

        public async Task<IActionResult> IhaleDetay(int id)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);

            if (ihale == null)
            {
                return NotFound();
            }

            return View(ihale);
        }

        public async Task<IActionResult> IhalePdfGoruntule(int id)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);

            if (ihale == null || string.IsNullOrEmpty(ihale.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ihale.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + ihale.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> IhalePdfIndir(int id)
        {
            var ihale = await _context.Ihaleler.FindAsync(id);

            if (ihale == null || string.IsNullOrEmpty(ihale.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ihale.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}




