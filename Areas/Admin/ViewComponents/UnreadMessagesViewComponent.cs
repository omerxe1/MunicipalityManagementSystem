using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;

namespace kocaaliv2.Areas.Admin.ViewComponents
{
    public class UnreadMessagesViewComponent : ViewComponent
    {
        private readonly KocaaliContext _context;

        public UnreadMessagesViewComponent(KocaaliContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new UnreadMessagesViewModel
            {
                OkunmamisIletisimBasvuru = await _context.IletisimBasvurulari.CountAsync(i => !i.OkunduMu),
                OkunmamisIletisimFormu = await _context.IletisimFormlari.CountAsync(i => !i.OkunduMu),
                OkunmamisBilgiEdinme = await _context.BilgiEdinmeBasvurulari.CountAsync(i => !i.OkunduMu),
                OkunmamisIstekVeOneri = await _context.IstekVeOneriler.CountAsync(i => !i.OkunduMu)
            };

            return View(model);
        }
    }

    public class UnreadMessagesViewModel
    {
        public int OkunmamisIletisimBasvuru { get; set; }
        public int OkunmamisIletisimFormu { get; set; }
        public int OkunmamisBilgiEdinme { get; set; }
        public int OkunmamisIstekVeOneri { get; set; }
    }
}





