using Microsoft.AspNetCore.Mvc;
using kocaaliv2.Data;
using kocaaliv2.Models;
using System.Security.Cryptography;
using System.Text;

namespace kocaaliv2.Controllers
{
    public class SeedController : Controller
    {
        private readonly KocaaliContext _context;

        public SeedController(KocaaliContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Örnek haber verilerini ekle
            if (!_context.Haberler.Any())
            {
                var haberler = new List<Haber>
                {
                    new Haber
                    {
                        Baslik = "Kocaali Belediyesi'nde Yeni Projeler Başladı",
                        Icerik = "Belediyemiz tarafından hayata geçirilen yeni projeler vatandaşlarımızın hizmetine sunuldu. Bu projeler kapsamında şehrimizin altyapısı güçlendirilecek ve vatandaşlarımızın yaşam kalitesi artırılacaktır.",
                        ResimYolu = "https://via.placeholder.com/400x250/4A90E2/FFFFFF?text=Haber+1",
                        Tarih = DateTime.Now.AddDays(-2)
                    },
                    new Haber
                    {
                        Baslik = "Kocaali'de Kültür ve Sanat Etkinlikleri",
                        Icerik = "Şehrimizde düzenlenen kültür ve sanat etkinlikleri büyük ilgi görüyor. Bu etkinlikler sayesinde vatandaşlarımız sanatla buluşuyor ve şehrimizin kültürel zenginliği artıyor.",
                        ResimYolu = "https://via.placeholder.com/400x250/7ED321/FFFFFF?text=Haber+2",
                        Tarih = DateTime.Now.AddDays(-4)
                    },
                    new Haber
                    {
                        Baslik = "Belediye Hizmetleri Dijitalleşiyor",
                        Icerik = "Vatandaşlarımız artık belediye hizmetlerine online olarak ulaşabilecek. Bu dijital dönüşüm sayesinde hizmetlerimiz daha hızlı ve kolay erişilebilir hale geliyor.",
                        ResimYolu = "https://via.placeholder.com/400x250/F5A623/FFFFFF?text=Haber+3",
                        Tarih = DateTime.Now.AddDays(-6)
                    },
                    new Haber
                    {
                        Baslik = "Çevre Dostu Projeler Hayata Geçiriliyor",
                        Icerik = "Belediyemiz çevre dostu projelerle şehrimizi daha yeşil ve temiz hale getiriyor. Bu projeler gelecek nesillere daha iyi bir çevre bırakmak için önemli adımlar.",
                        ResimYolu = "https://via.placeholder.com/400x250/50C878/FFFFFF?text=Haber+4",
                        Tarih = DateTime.Now.AddDays(-8)
                    },
                    new Haber
                    {
                        Baslik = "Sosyal Destek Programları Genişliyor",
                        Icerik = "Vatandaşlarımıza yönelik sosyal destek programları genişletiliyor. Bu programlar sayesinde ihtiyaç sahibi vatandaşlarımıza daha kapsamlı destek sağlanıyor.",
                        ResimYolu = "https://via.placeholder.com/400x250/FF6B6B/FFFFFF?text=Haber+5",
                        Tarih = DateTime.Now.AddDays(-10)
                    }
                };

                _context.Haberler.AddRange(haberler);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Test için admin kullanıcısı oluşturur
        /// Kullanıcı adı: admin, Şifre: admin123
        /// </summary>
        public IActionResult CreateAdmin()
        {
            if (!_context.Admins.Any(a => a.KullaniciAdi == "admin"))
            {
                var admin = new Admin
                {
                    KullaniciAdi = "admin",
                    SifreHash = HashSifre("admin123"),
                    Rol = "Admin",
                    AktifMi = true
                };

                _context.Admins.Add(admin);
                _context.SaveChanges();

                return Content("Admin kullanıcısı oluşturuldu!<br>Kullanıcı Adı: admin<br>Şifre: admin123<br><br><a href='/yonetim-portal'>Admin Paneline Git</a>", "text/html");
            }

            return Content("Admin kullanıcısı zaten mevcut!<br><br><a href='/yonetim-portal'>Admin Paneline Git</a>", "text/html");
        }

        /// <summary>
        /// Şifreyi SHA256 ile hash'ler
        /// </summary>
        private string HashSifre(string sifre)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(sifre);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}





































