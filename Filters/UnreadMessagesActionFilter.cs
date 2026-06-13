using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;

namespace kocaaliv2.Filters
{
    public class UnreadMessagesActionFilter : IAsyncActionFilter
    {
        private readonly KocaaliContext _context;

        public UnreadMessagesActionFilter(KocaaliContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Action çalışmadan önce okunmamış mesaj sayılarını hesapla
            var okunmamisIletisimBasvuru = await _context.IletisimBasvurulari.CountAsync(i => !i.OkunduMu);
            var okunmamisIletisimFormu = await _context.IletisimFormlari.CountAsync(i => !i.OkunduMu);
            var okunmamisBilgiEdinme = await _context.BilgiEdinmeBasvurulari.CountAsync(i => !i.OkunduMu);
            var okunmamisIstekVeOneri = await _context.IstekVeOneriler.CountAsync(i => !i.OkunduMu);

            // ViewBag'e set et
            if (context.Controller is Controller controller)
            {
                controller.ViewBag.OkunmamisIletisimBasvuru = okunmamisIletisimBasvuru;
                controller.ViewBag.OkunmamisIletisimFormu = okunmamisIletisimFormu;
                controller.ViewBag.OkunmamisBilgiEdinme = okunmamisBilgiEdinme;
                controller.ViewBag.OkunmamisIstekVeOneri = okunmamisIstekVeOneri;
            }

            await next();
        }
    }
}





