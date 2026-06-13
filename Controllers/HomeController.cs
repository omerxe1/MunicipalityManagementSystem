using Microsoft.AspNetCore.Mvc;

namespace kocaaliv2.Controllers
{
    /// <summary>
    /// Ana sayfa ve hata sayfaları için controller
    /// FAZ 4: Production ortamı için özel error handling
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Hata sayfası - Production ortamı için
        /// </summary>
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                ViewBag.StatusCode = statusCode.Value;
                
                switch (statusCode.Value)
                {
                    case 403:
                        ViewBag.ErrorMessage = "Bu sayfaya erişim yetkiniz yok.";
                        break;
                    case 404:
                        ViewBag.ErrorMessage = "Aradığınız sayfa bulunamadı.";
                        break;
                    case 500:
                        ViewBag.ErrorMessage = "Sunucu hatası oluştu.";
                        break;
                    default:
                        ViewBag.ErrorMessage = "Bir hata oluştu.";
                        break;
                }
            }
            else
            {
                ViewBag.StatusCode = 500;
                ViewBag.ErrorMessage = "Bir hata oluştu.";
            }

            return View();
        }
    }
}
