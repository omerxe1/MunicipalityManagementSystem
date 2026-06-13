using Microsoft.AspNetCore.Mvc;

namespace kocaaliv2.Controllers
{
    public class MudurluklerController : Controller
    {
        // Ana müdürlükler listesi sayfası
        public IActionResult Index()
        {
            return RedirectToAction("Mudurlukler", "Kurumsal");
        }

        // Basın Yayın ve Halkla İlişkiler Müdürlüğü
        public IActionResult BasinYayinHalklaIliskiler()
        {
            return View();
        }

        // Kültür ve Sosyal İşler Müdürlüğü
        public IActionResult KulturVeSosyalIsler()
        {
            return View();
        }

        // Fen İşleri Müdürlüğü
        public IActionResult FenIsleri()
        {
            return View();
        }

        // Mali Hizmetler Müdürlüğü
        public IActionResult MaliHizmetler()
        {
            return View();
        }

        // Ruhsat ve Denetim Müdürlüğü
        public IActionResult RuhsatVeDenetim()
        {
            return View();
        }

        // Yazı İşleri Müdürlüğü
        public IActionResult YaziIsleri()
        {
            return View();
        }

        // Zabıta Müdürlüğü
        public IActionResult Zabita()
        {
            return View();
        }

        // Bilgi İşlem Müdürlüğü
        public IActionResult BilgiIslem()
        {
            return View();
        }

        // Çevre Koruma ve Kontrol Müdürlüğü
        public IActionResult CevreKorumaVeKontrol()
        {
            return View();
        }

        // İmar ve Şehircilik Müdürlüğü
        public IActionResult ImarVeSehircilik()
        {
            return View();
        }
    }
}



















