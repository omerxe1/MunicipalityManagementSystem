using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;

namespace kocaaliv2.Controllers
{
    public class PlanlarRaporlarController : Controller
    {
        private readonly KocaaliContext _context;

        public PlanlarRaporlarController(KocaaliContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> FaaliyetRaporlari()
        {
            var faaliyetRaporlari = await _context.FaaliyetRaporlari
                .Where(f => f.AktifMi == true)
                .OrderByDescending(f => f.Yil)
                .ThenByDescending(f => f.OlusturmaTarihi)
                .ToListAsync();

            return View(faaliyetRaporlari);
        }

        public async Task<IActionResult> FaaliyetRaporuPdfGoruntule(int id)
        {
            var rapor = await _context.FaaliyetRaporlari.FindAsync(id);

            if (rapor == null || string.IsNullOrEmpty(rapor.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rapor.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + rapor.Yil + "_Faaliyet_Raporu.pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> FaaliyetRaporuPdfIndir(int id)
        {
            var rapor = await _context.FaaliyetRaporlari.FindAsync(id);

            if (rapor == null || string.IsNullOrEmpty(rapor.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", rapor.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"{rapor.Yil}_Faaliyet_Raporu.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> ImarPlanlari(int page = 1)
        {
            int pageSize = 10;
            int skip = (page - 1) * pageSize;

            var totalImarPlanlari = await _context.ImarPlanlari
                .Where(i => i.AktifMi == true)
                .CountAsync();

            var imarPlanlari = await _context.ImarPlanlari
                .Where(i => i.AktifMi == true)
                .OrderByDescending(i => i.OlusturmaTarihi)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalImarPlanlari / (double)pageSize);
            ViewBag.TotalImarPlanlari = totalImarPlanlari;

            return View(imarPlanlari);
        }

        public async Task<IActionResult> ImarPlaniDetay(int id)
        {
            var imarPlani = await _context.ImarPlanlari.FindAsync(id);

            if (imarPlani == null)
            {
                return NotFound();
            }

            return View(imarPlani);
        }

        public async Task<IActionResult> ImarPlaniPdfGoruntule(int id)
        {
            var imarPlani = await _context.ImarPlanlari.FindAsync(id);

            if (imarPlani == null || string.IsNullOrEmpty(imarPlani.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imarPlani.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + imarPlani.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> ImarPlaniPdfIndir(int id)
        {
            var imarPlani = await _context.ImarPlanlari.FindAsync(id);

            if (imarPlani == null || string.IsNullOrEmpty(imarPlani.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imarPlani.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> IcKontrolUyumEylemPlanlari()
        {
            var planlar = await _context.IcKontrolUyumEylemPlanlari
                .Where(i => i.AktifMi == true)
                .OrderBy(i => i.SiraNo)
                .ThenByDescending(i => i.OlusturmaTarihi)
                .ToListAsync();

            return View(planlar);
        }

        public async Task<IActionResult> IcKontrolUyumEylemPlaniPdfGoruntule(int id)
        {
            var plan = await _context.IcKontrolUyumEylemPlanlari.FindAsync(id);

            if (plan == null || string.IsNullOrEmpty(plan.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", plan.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + plan.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> IcKontrolUyumEylemPlaniPdfIndir(int id)
        {
            var plan = await _context.IcKontrolUyumEylemPlanlari.FindAsync(id);

            if (plan == null || string.IsNullOrEmpty(plan.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", plan.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> PerformansProgrami()
        {
            var performansProgramlari = await _context.PerformansProgramlari
                .Where(p => p.AktifMi == true)
                .OrderByDescending(p => p.MaliYil)
                .ThenByDescending(p => p.OlusturmaTarihi)
                .ToListAsync();

            return View(performansProgramlari);
        }

        public async Task<IActionResult> PerformansProgramiPdfGoruntule(int id)
        {
            var program = await _context.PerformansProgramlari.FindAsync(id);

            if (program == null || string.IsNullOrEmpty(program.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", program.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + program.MaliYil + "_Performans_Programi.pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> PerformansProgramiPdfIndir(int id)
        {
            var program = await _context.PerformansProgramlari.FindAsync(id);

            if (program == null || string.IsNullOrEmpty(program.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", program.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"{program.MaliYil}_Performans_Programi.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> StratejikPlanlar()
        {
            var stratejikPlanlar = await _context.StratejikPlanlar
                .Where(s => s.AktifMi == true)
                .OrderBy(s => s.SiraNo)
                .ThenByDescending(s => s.OlusturmaTarihi)
                .ToListAsync();

            return View(stratejikPlanlar);
        }

        public async Task<IActionResult> StratejikPlanPdfGoruntule(int id)
        {
            var plan = await _context.StratejikPlanlar.FindAsync(id);

            if (plan == null || string.IsNullOrEmpty(plan.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", plan.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + plan.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> StratejikPlanPdfIndir(int id)
        {
            var plan = await _context.StratejikPlanlar.FindAsync(id);

            if (plan == null || string.IsNullOrEmpty(plan.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", plan.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> KaliteYonetimSistemi()
        {
            var kaliteYonetimSistemleri = await _context.KaliteYonetimSistemleri
                .Where(k => k.AktifMi == true)
                .OrderBy(k => k.SiraNo)
                .ThenByDescending(k => k.OlusturmaTarihi)
                .ToListAsync();

            return View(kaliteYonetimSistemleri);
        }

        public async Task<IActionResult> KaliteYonetimSistemiPdfGoruntule(int id)
        {
            var kalite = await _context.KaliteYonetimSistemleri.FindAsync(id);

            if (kalite == null || string.IsNullOrEmpty(kalite.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", kalite.Url.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + kalite.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> KaliteYonetimSistemiPdfIndir(int id)
        {
            var kalite = await _context.KaliteYonetimSistemleri.FindAsync(id);

            if (kalite == null || string.IsNullOrEmpty(kalite.Url))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", kalite.Url.TrimStart('/'));

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

