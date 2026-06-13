using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Filters;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{

    [Area("Admin")]
    [AdminAuthorize]
    public class DashboardController : BaseAdminController
    {
        private readonly KocaaliContext _context;

        public DashboardController(KocaaliContext context, IAdminLogService adminLogService)
            : base(adminLogService)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.KullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi");
            ViewBag.Rol = HttpContext.Session.GetString("AdminRol");
            var okunmamisIstekVeOneriSayisi = await _context.IstekVeOneriler
                .CountAsync(i => !i.OkunduMu);
            ViewBag.OkunmamisIstekVeOneriSayisi = okunmamisIstekVeOneriSayisi;
            var bugun = DateTime.Now.Date;
            var yaklasanEtkinlikSayisi = await _context.Etkinlikler
                .CountAsync(e => (e.Tarih >= bugun || e.BaslangicTarihi >= bugun) && e.YayindaMi);
            ViewBag.YaklasanEtkinlikSayisi = yaklasanEtkinlikSayisi;

            return View();
        }
    }
}


