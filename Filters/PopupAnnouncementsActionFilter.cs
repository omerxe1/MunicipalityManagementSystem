using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;

namespace kocaaliv2.Filters
{
    /// <summary>
    /// Popup duyuruları tüm sayfalara yükleyen action filter
    /// </summary>
    public class PopupAnnouncementsActionFilter : IAsyncActionFilter
    {
        private readonly KocaaliContext _context;

        public PopupAnnouncementsActionFilter(KocaaliContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Admin area'daki sayfalarda popup duyuruları yükleme
            if (!context.RouteData.Values.ContainsKey("area") || 
                context.RouteData.Values["area"]?.ToString() != "Admin")
            {
                // Sadece aktif popup duyuruları yükle
                var popupAnnouncements = await _context.PopupAnnouncements
                    .Where(p => p.AktifMi)
                    .OrderBy(p => p.SiraNo)
                    .ToListAsync();

                // ViewBag'e set et
                if (context.Controller is Controller controller)
                {
                    controller.ViewBag.PopupAnnouncements = popupAnnouncements;
                }
            }

            await next();
        }
    }
}



