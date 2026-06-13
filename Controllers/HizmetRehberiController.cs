using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Controllers
{
    public class HizmetRehberiController : Controller
    {
        private readonly KocaaliContext _context;

        public HizmetRehberiController(KocaaliContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Projeler()
        {
            ViewData["PageTitle"] = "Projeler";
            ViewData["ContentTitle"] = "PROJELER";
            ViewBag.ShowBackButton = true;

            return View();
        }

        public async Task<IActionResult> ProjeListesi(string? durum = null)
        {
            var query = _context.Calismalar.AsQueryable();

            if (!string.IsNullOrEmpty(durum))
            {
                query = query.Where(c => c.Durum == durum);
            }

            var projeler = await query
                .OrderByDescending(c => c.PublishDate)
                .ToListAsync();

            var baslik = durum switch
            {
                "Devam Eden" => "Devam Eden Projeler",
                "Tamamlanan" => "Tamamlanan Projeler",
                "Planlanan" => "Planlanan Projeler",
                _ => "Projeler"
            };

            ViewData["PageTitle"] = baslik;
            ViewData["ContentTitle"] = baslik.ToUpper();
            ViewBag.ShowBackButton = true;
            ViewBag.SeciliDurum = durum;

            return View(projeler);
        }

        public async Task<IActionResult> BasvuruKilavuzu()
        {
            ViewData["PageTitle"] = "Başvuru Kılavuzları";
            ViewData["ContentTitle"] = "BAŞVURU KLAVUZLARI";
            ViewBag.ShowBackButton = true;
            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İçerikler", Url = (string?)"#" },
                new { Text = "BAŞVURU KLAVUZLARI", Url = (string?)null }
            };

            var kilavuzlar = await _context.BasvuruKilavuzlari
                .Where(k => k.AktifMi)
                .OrderBy(k => k.Sira)
                .ThenBy(k => k.Baslik)
                .ToListAsync();

            return View(kilavuzlar);
        }

        public async Task<IActionResult> BasvuruKilavuzuDetay(int id)
        {
            var kilavuz = await _context.BasvuruKilavuzlari
                .FirstOrDefaultAsync(k => k.Id == id && k.AktifMi);

            if (kilavuz == null)
            {
                return NotFound();
            }

            ViewData["PageTitle"] = kilavuz.Baslik;
            ViewData["ContentTitle"] = kilavuz.Baslik.ToUpper();
            ViewBag.ShowBackButton = true;

            // Breadcrumb oluştur
            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İçerikler", Url = (string?)"#" },
                new { Text = "BAŞVURU KLAVUZLARI", Url = (string?)(Url.Action("BasvuruKilavuzu", "HizmetRehberi") ?? "#") },
                new { Text = kilavuz.Baslik, Url = (string?)null }
            };

            return View(kilavuz);
        }

        public async Task<IActionResult> ProjeDetay(int id)
        {
            var proje = await _context.Calismalar
                .FirstOrDefaultAsync(c => c.Id == id);

            if (proje == null)
            {
                return NotFound();
            }

            ViewData["PageTitle"] = proje.Title;
            ViewData["ContentTitle"] = proje.Title.ToUpper();
            ViewBag.ShowBackButton = true;

            // Breadcrumb oluştur
            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "Hizmet Rehberi", Url = (string?)"#" },
                new { Text = "Projeler", Url = (string?)(Url.Action("Projeler", "HizmetRehberi") ?? "#") },
                new { Text = proje.Title, Url = (string?)null }
            };

            return View(proje);
        }
    }
}

