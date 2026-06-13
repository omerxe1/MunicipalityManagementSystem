using Microsoft.AspNetCore.Mvc;
using System.Linq;
using kocaaliv2.Data;

namespace kocaaliv2.Controllers
{
    public class HaberlerController : Controller
    {
        private readonly KocaaliContext _context;

        public HaberlerController(KocaaliContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var haberler = _context.Haberler.OrderByDescending(h => h.Tarih).ToList();
            return View(haberler);
        }
    }
}

