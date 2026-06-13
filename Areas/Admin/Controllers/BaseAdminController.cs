using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using kocaaliv2.Data;
using kocaaliv2.Services;

namespace kocaaliv2.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public abstract class BaseAdminController : Controller
    {
        protected readonly IAdminLogService _adminLogService;

        protected BaseAdminController(IAdminLogService adminLogService)
        {
            _adminLogService = adminLogService;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbContext = HttpContext.RequestServices.GetService<KocaaliContext>();
            if (dbContext != null)
            {
                ViewBag.OkunmamisIletisimBasvuru = await dbContext.IletisimBasvurulari.CountAsync(i => !i.OkunduMu);
                ViewBag.OkunmamisIletisimFormu = await dbContext.IletisimFormlari.CountAsync(i => !i.OkunduMu);
                ViewBag.OkunmamisBilgiEdinme = await dbContext.BilgiEdinmeBasvurulari.CountAsync(i => !i.OkunduMu);
                ViewBag.OkunmamisIstekVeOneri = await dbContext.IstekVeOneriler.CountAsync(i => !i.OkunduMu);
            }

            await base.OnActionExecutionAsync(context, next);
        }

        protected string GetClientIP()
        {
            var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            var realIP = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        protected async Task LogAsync(string islem, string? tabloAdi, string? aciklama)
        {
            var kullaniciAdi = HttpContext.Session.GetString("AdminKullaniciAdi") ?? "System";
            var clientIP = GetClientIP();
            await _adminLogService.LogAsync(kullaniciAdi, islem, tabloAdi, aciklama, clientIP);
        }
    }
}


