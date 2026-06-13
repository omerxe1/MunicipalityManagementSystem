using Microsoft.AspNetCore.Mvc;
using System.Linq;
using kocaaliv2.Data;

namespace kocaaliv2.Controllers
{
    public class EtkinlikController : Controller
    {
        private readonly KocaaliContext _context;

        public EtkinlikController(KocaaliContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
           
            return View();
        }
    }
}










