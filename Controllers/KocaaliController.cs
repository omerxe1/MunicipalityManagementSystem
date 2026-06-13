using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using Microsoft.AspNetCore.Hosting;

namespace kocaaliv2.Controllers
{
    public class KocaaliController : Controller
    {
        private readonly KocaaliContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KocaaliController(KocaaliContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Hakkinda()
        {
            return View();
        }

        public IActionResult Tarihce()
        {
            return View();
        }

        public IActionResult IklimVeCografya()
        {
            return View();
        }

        public async Task<IActionResult> KardesSehirlerimiz()
        {
            var kardesSehirler = await _context.KardesSehirler
                .Where(k => k.AktifMi == true)
                .OrderBy(k => k.SiraNo)
                .ToListAsync();

            return View(kardesSehirler);
        }

        public IActionResult TarihVeTurizmHaritasi()
        {
            return View();
        }

        public IActionResult Galeri()
        {
            return View();
        }

        public IActionResult Fotograflar()
        {
            var fotoKlasoru = Path.Combine(_webHostEnvironment.WebRootPath, "images", "KocaaliFotolar");
            var fotograflar = new List<string>();

            if (Directory.Exists(fotoKlasoru))
            {
                var dosyalar = Directory.GetFiles(fotoKlasoru)
                    .Where(f => f.ToLower().EndsWith(".jpg") || f.ToLower().EndsWith(".jpeg") || f.ToLower().EndsWith(".png"))
                    .Select(f => Path.GetFileName(f))
                    .OrderBy(f => f)
                    .ToList();

                fotograflar = dosyalar;
            }

            return View(fotograflar);
        }
    }
}

