using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Controllers
{
    public class KurumsalController : Controller
    {
        private readonly KocaaliContext _context;

        public KurumsalController(KocaaliContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Baskan()
        {
            var photos = await _context.Photos.OrderByDescending(p => p.UploadDate).ToListAsync();
            ViewBag.Photos = photos;
            return View();
        }

        public async Task<IActionResult> BaskanFotograflari()
        {
            var photos = await _context.Photos.OrderByDescending(p => p.UploadDate).ToListAsync();
            return View(photos);
        }

        public IActionResult BaskanMesaj()
        {
            return View();
        }

        public IActionResult KurumsalYapi()
        {
            return View();
        }

        public IActionResult BaskanYardimcilari()
        {
            return View();
        }

        public IActionResult BelediyeEncumenleri()
        {
            return View();
        }

        public IActionResult Mudurlukler()
        {
            return View();
        }

        public async Task<IActionResult> Yonetmelikler()
        {
            var yonetmelikler = await _context.Yonetmelikler
                .Where(y => y.AktifMi == true)
                .OrderBy(y => y.SiraNo)
                .ToListAsync();
            
            return View(yonetmelikler);
        }

        public async Task<IActionResult> EskiBaskanlar()
        {
            var eskiBaskanlar = await _context.EskiBaskanlar
                .Where(e => e.AktifMi == true)
                .OrderBy(e => e.SiraNo)
                .ToListAsync();
            
            return View(eskiBaskanlar);
        }

        public async Task<IActionResult> MeclisUyeleri(string? parti = null)
        {
            var meclisUyeleri = await _context.MeclisUyeleri
                .Where(m => m.AktifMi == true)
                .OrderBy(m => m.SiraNo)
                .ToListAsync();

            // Parti eşleştirmesi için esnek kontrol
            var partiEslesmeleri = new Dictionary<string, List<string>>
            {
                { "AK PARTİ", new List<string> { "AK PARTİ", "AK Parti", "Adalet ve Kalkınma Partisi", "Adalet Ve Kalkınma Partisi" } },
                { "MİLLİYETÇİ HAREKET PARTİSİ", new List<string> { "MHP", "MİLLİYETÇİ HAREKET PARTİSİ", "Milliyetçi Hareket Partisi", "Milliyetci Hareket Partisi" } },
                { "YENİDEN REFAH", new List<string> { "YENİDEN REFAH", "Yeniden Refah Partisi", "Yeniden Refah", "Yeniden Refah Partisi" } },
                { "SAADET PARTİSİ", new List<string> { "SAADET PARTİSİ", "Saadet Partisi", "Saadet Partisi" } },
                { "İYİ PARTİ", new List<string> { "İYİ PARTİ", "İyi Parti", "İYİ Parti", "İyi Parti" } }
            };

            // Seçili partiye göre filtrele
            if (!string.IsNullOrEmpty(parti))
            {
                var eslesenPartiler = partiEslesmeleri.ContainsKey(parti) 
                    ? partiEslesmeleri[parti] 
                    : new List<string> { parti };

                meclisUyeleri = meclisUyeleri
                    .Where(m => eslesenPartiler.Any(p => m.Parti.Equals(p, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            ViewBag.SeciliParti = parti;

            return View(meclisUyeleri);
        }

        public async Task<IActionResult> MeclisKararlari(int? ay = null, int? yil = null)
        {
            var meclisKararlari = await _context.MeclisKararlari
                .Where(m => m.AktifMi == true)
                .OrderByDescending(m => m.Tarih)
                .ToListAsync();

            // Ay ve yıl filtreleme
            if (ay.HasValue && yil.HasValue)
            {
                meclisKararlari = meclisKararlari
                    .Where(m => m.Tarih.Year == yil.Value && m.Tarih.Month == ay.Value)
                    .ToList();
            }
            else if (yil.HasValue)
            {
                meclisKararlari = meclisKararlari
                    .Where(m => m.Tarih.Year == yil.Value)
                    .ToList();
            }
            else if (ay.HasValue)
            {
                meclisKararlari = meclisKararlari
                    .Where(m => m.Tarih.Month == ay.Value)
                    .ToList();
            }

            // Mevcut yılları al (dropdown için)
            var mevcutYillar = await _context.MeclisKararlari
                .Where(m => m.AktifMi == true)
                .Select(m => m.Tarih.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            ViewBag.MevcutYillar = mevcutYillar;
            ViewBag.SeciliAy = ay;
            ViewBag.SeciliYil = yil;

            return View(meclisKararlari);
        }

        public IActionResult IcKontrol()
        {
            return View();
        }

        public IActionResult MisyonVizyon()
        {
            return View();
        }

        public IActionResult OrganizasyonSemasi()
        {
            return View();
        }

        public async Task<IActionResult> Kvkk()
        {
            var kvkkDokumanlari = await _context.KvkkDokumanlari
                .Where(k => k.AktifMi)
                .OrderBy(k => k.SiraNo)
                .ThenByDescending(k => k.OlusturmaTarihi)
                .ToListAsync();

            return View(kvkkDokumanlari);
        }

        public async Task<IActionResult> KvkkPdfGoruntule(int id)
        {
            var dokuman = await _context.KvkkDokumanlari.FindAsync(id);

            if (dokuman == null || string.IsNullOrEmpty(dokuman.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dokuman.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // PDF'i inline olarak göster (indirme yerine görüntüleme)
            Response.Headers["Content-Disposition"] = "inline; filename=\"" + dokuman.Baslik.Replace(" ", "_") + ".pdf\"";
            
            return File(fileBytes, "application/pdf");
        }

        public async Task<IActionResult> KvkkPdfIndir(int id)
        {
            var dokuman = await _context.KvkkDokumanlari.FindAsync(id);

            if (dokuman == null || string.IsNullOrEmpty(dokuman.PdfUrl))
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dokuman.PdfUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"{dokuman.Baslik.Replace(" ", "_")}.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
