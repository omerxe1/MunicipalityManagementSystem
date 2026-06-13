using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Controllers
{
    public class PhotosController : Controller
    {
        private readonly KocaaliContext _context;

        public PhotosController(KocaaliContext context)
        {
            _context = context;
        }

        // GET: Photos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Photos.OrderByDescending(p => p.UploadDate).ToListAsync());
        }
    }
}

