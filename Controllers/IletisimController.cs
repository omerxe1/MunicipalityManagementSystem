using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Controllers
{
    public class IletisimController : Controller
    {
        private readonly KocaaliContext _context;

        public IletisimController(KocaaliContext context)
        {
            _context = context;
        }
        public IActionResult BizeUlasin()
        {
            ViewData["PageTitle"] = "Bize Ulaşın";
            ViewData["ContentTitle"] = "BİZE ULAŞIN";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Bize Ulaşın", Url = (string?)null }
            };

            return View();
        }

        [HttpPost]
        public IActionResult BizeUlasinGonder(string KimlikNo, int DogumGunu, int DogumAyi, string DogumYili, 
            string Ad, string Soyad, string CepTelefonu, string Email, string BasvuruMetni, 
            bool KisiBilgilerimiGizle, string CaptchaCode)
        {
            if (string.IsNullOrEmpty(BasvuruMetni))
            {
                TempData["ErrorMessage"] = "Lütfen başvuru metnini doldurun.";
                return RedirectToAction("BizeUlasin");
            }

            if (!KisiBilgilerimiGizle)
            {
                if (string.IsNullOrEmpty(KimlikNo) || string.IsNullOrEmpty(Ad) || string.IsNullOrEmpty(Soyad) 
                    || string.IsNullOrEmpty(CepTelefonu))
                {
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun.";
                    return RedirectToAction("BizeUlasin");
                }
            }

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(CaptchaCode) || string.IsNullOrEmpty(sessionCaptcha))
            {
                TempData["ErrorMessage"] = "Lütfen captcha kodunu girin.";
                return RedirectToAction("BizeUlasin");
            }

            if (CaptchaCode.ToUpper() != sessionCaptcha.ToUpper())
            {
                TempData["ErrorMessage"] = "Captcha kodu hatalı. Lütfen tekrar deneyin.";
                HttpContext.Session.Remove("CaptchaCode");
                return RedirectToAction("BizeUlasin");
            }

            HttpContext.Session.Remove("CaptchaCode");

            var iletisimFormu = new IletisimFormu
            {
                KimlikNo = KisiBilgilerimiGizle ? "bilinmiyor" : KimlikNo,
                DogumGunu = KisiBilgilerimiGizle ? 0 : DogumGunu,
                DogumAyi = KisiBilgilerimiGizle ? 0 : DogumAyi,
                DogumYili = KisiBilgilerimiGizle ? "bilinmiyor" : DogumYili,
                Ad = KisiBilgilerimiGizle ? "bilinmiyor" : Ad,
                Soyad = KisiBilgilerimiGizle ? "bilinmiyor" : Soyad,
                CepTelefonu = KisiBilgilerimiGizle ? "bilinmiyor" : CepTelefonu,
                Email = KisiBilgilerimiGizle ? "bilinmiyor" : Email,
                BasvuruMetni = BasvuruMetni,
                KisiBilgilerimiGizle = KisiBilgilerimiGizle,
                OlusturmaTarihi = DateTime.Now,
                OkunduMu = false
            };

            try
            {
                _context.IletisimFormlari.Add(iletisimFormu);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Başvurunuz başarıyla alınmıştır. En kısa sürede size dönüş yapacağız.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Başvuru kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
                if (HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
                {
                    TempData["ErrorMessage"] += $" Hata: {ex.Message}";
                }
            }

            return RedirectToAction("BizeUlasin");
        }

        [HttpGet]
        public IActionResult Captcha()
        {
            var random = new Random();
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var captchaCode = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            HttpContext.Session.SetString("CaptchaCode", captchaCode);

            using (var bitmap = new Bitmap(150, 50))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);

                var pen = new Pen(Color.LightGray, 1);
                for (int i = 0; i < 10; i++)
                {
                    var x1 = random.Next(0, 150);
                    var y1 = random.Next(0, 50);
                    var x2 = random.Next(0, 150);
                    var y2 = random.Next(0, 50);
                    graphics.DrawLine(pen, x1, y1, x2, y2);
                }

                for (int i = 0; i < 50; i++)
                {
                    var x = random.Next(0, 150);
                    var y = random.Next(0, 50);
                    bitmap.SetPixel(x, y, Color.Gray);
                }

                var font = new Font("Arial", 20, FontStyle.Bold);
                var colors = new[] { Color.Blue, Color.Green, Color.Purple, Color.Red, Color.Orange };
                
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    var brush = new SolidBrush(colors[random.Next(colors.Length)]);
                    var x = 10 + (i * 25) + random.Next(-3, 3);
                    var y = 10 + random.Next(-5, 5);
                    graphics.DrawString(captchaCode[i].ToString(), font, brush, x, y);
                }

                var linePen = new Pen(Color.Black, 2);
                graphics.DrawLine(linePen, 0, 25, 150, 25);

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }

        public IActionResult BilgiEdinme(bool clearSession = false)
        {
            ViewData["PageTitle"] = "Bilgi Edinme";
            ViewData["ContentTitle"] = "BİLGİ EDİNME";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Bilgi Edinme", Url = (string?)null }
            };

            if (clearSession)
            {
                HttpContext.Session.Remove("TakipBasvuruIds");
                HttpContext.Session.Remove("TakipCepTelefonu");
                return RedirectToAction("BilgiEdinme");
            }

            var basvuruIdsStr = HttpContext.Session.GetString("TakipBasvuruIds");
            if (!string.IsNullOrEmpty(basvuruIdsStr))
            {
                var basvuruIds = basvuruIdsStr.Split(',').Select(int.Parse).ToList();
                var basvurular = _context.BilgiEdinmeBasvurulari
                    .Where(b => basvuruIds.Contains(b.Id))
                    .OrderByDescending(b => b.OlusturmaTarihi)
                    .ToList();
                ViewBag.Basvurular = basvurular;
            }

            return View();
        }

        [HttpPost]
        public IActionResult BilgiEdinmeGonder(string SahisTuru, string CaptchaCode, 
            string KimlikNo, int? DogumGunu, int? DogumAyi, string DogumYili,
            string Ad, string Soyad, string EvTelefonu, string CepTelefonu, 
            string Email, string IletisimAdresi, string BasvuruMetni, string CevapNasilVerilsin,
            string Unvan, string VergiDairesi, string VergiNo, string IsTelefonu,
            string TuzelCepTelefonu, string TuzelEmail, string TuzelIletisimAdresi,
            string TuzelBasvuruMetni, string TuzelCevapNasilVerilsin)
        {
            if (string.IsNullOrEmpty(SahisTuru) || (SahisTuru != "Sahis" && SahisTuru != "Tuzel"))
            {
                TempData["ErrorMessage"] = "Lütfen şahıs türünü seçiniz.";
                return RedirectToAction("BilgiEdinme");
            }

            if (SahisTuru == "Sahis")
            {
                if (string.IsNullOrEmpty(KimlikNo) || !DogumGunu.HasValue || !DogumAyi.HasValue || 
                    string.IsNullOrEmpty(DogumYili) || string.IsNullOrEmpty(Ad) || 
                    string.IsNullOrEmpty(Soyad) || string.IsNullOrEmpty(CepTelefonu) || 
                    string.IsNullOrEmpty(Email))
                {
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun.";
                    return RedirectToAction("BilgiEdinme");
                }
            }

            if (SahisTuru == "Tuzel")
            {
                if (string.IsNullOrEmpty(Unvan) || string.IsNullOrEmpty(VergiDairesi) || 
                    string.IsNullOrEmpty(VergiNo) || string.IsNullOrEmpty(TuzelCepTelefonu) || 
                    string.IsNullOrEmpty(TuzelEmail))
                {
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun.";
                    return RedirectToAction("BilgiEdinme");
                }
            }

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(CaptchaCode) || string.IsNullOrEmpty(sessionCaptcha))
            {
                TempData["ErrorMessage"] = "Lütfen captcha kodunu girin.";
                return RedirectToAction("BilgiEdinme");
            }

            if (CaptchaCode.ToUpper() != sessionCaptcha.ToUpper())
            {
                TempData["ErrorMessage"] = "Captcha kodu hatalı. Lütfen tekrar deneyin.";
                HttpContext.Session.Remove("CaptchaCode");
                return RedirectToAction("BilgiEdinme");
            }

            HttpContext.Session.Remove("CaptchaCode");

            var bilgiEdinmeBasvuru = new BilgiEdinmeBasvuru
            {
                SahisTuru = SahisTuru,
                OlusturmaTarihi = DateTime.Now,
                OkunduMu = false
            };

            if (SahisTuru == "Sahis")
            {
                bilgiEdinmeBasvuru.KimlikNo = KimlikNo;
                bilgiEdinmeBasvuru.DogumGunu = DogumGunu;
                bilgiEdinmeBasvuru.DogumAyi = DogumAyi;
                bilgiEdinmeBasvuru.DogumYili = DogumYili;
                bilgiEdinmeBasvuru.Ad = Ad;
                bilgiEdinmeBasvuru.Soyad = Soyad;
                bilgiEdinmeBasvuru.EvTelefonu = EvTelefonu;
                bilgiEdinmeBasvuru.CepTelefonu = CepTelefonu;
                bilgiEdinmeBasvuru.Email = Email;
                bilgiEdinmeBasvuru.IletisimAdresi = IletisimAdresi;
                bilgiEdinmeBasvuru.BasvuruMetni = BasvuruMetni;
                bilgiEdinmeBasvuru.CevapNasilVerilsin = CevapNasilVerilsin;
            }
            else if (SahisTuru == "Tuzel")
            {
                bilgiEdinmeBasvuru.Unvan = Unvan;
                bilgiEdinmeBasvuru.VergiDairesi = VergiDairesi;
                bilgiEdinmeBasvuru.VergiNo = VergiNo;
                bilgiEdinmeBasvuru.IsTelefonu = IsTelefonu;
                bilgiEdinmeBasvuru.TuzelCepTelefonu = TuzelCepTelefonu;
                bilgiEdinmeBasvuru.TuzelEmail = TuzelEmail;
                bilgiEdinmeBasvuru.TuzelIletisimAdresi = TuzelIletisimAdresi;
                bilgiEdinmeBasvuru.TuzelBasvuruMetni = TuzelBasvuruMetni;
                bilgiEdinmeBasvuru.TuzelCevapNasilVerilsin = TuzelCevapNasilVerilsin;
            }

            try
            {
                _context.BilgiEdinmeBasvurulari.Add(bilgiEdinmeBasvuru);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Başvurunuz başarıyla alınmıştır. En kısa sürede size dönüş yapacağız.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Başvuru kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
                if (HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
                {
                    TempData["ErrorMessage"] += $" Hata: {ex.Message}";
                }
            }

            return RedirectToAction("BilgiEdinme");
        }

        [HttpPost]
        public IActionResult BasvuruTakipSorgula(string TakipCepTelefonu, string TakipCaptchaCode)
        {
            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(TakipCaptchaCode) || string.IsNullOrEmpty(sessionCaptcha))
            {
                TempData["TakipErrorMessage"] = "Lütfen captcha kodunu girin.";
                return RedirectToAction("BilgiEdinme");
            }

            if (TakipCaptchaCode.ToUpper() != sessionCaptcha.ToUpper())
            {
                TempData["TakipErrorMessage"] = "Captcha kodu hatalı. Lütfen tekrar deneyin.";
                HttpContext.Session.Remove("CaptchaCode");
                return RedirectToAction("BilgiEdinme");
            }

            HttpContext.Session.Remove("CaptchaCode");

            if (string.IsNullOrEmpty(TakipCepTelefonu))
            {
                TempData["TakipErrorMessage"] = "Lütfen cep telefonu numaranızı giriniz.";
                return RedirectToAction("BilgiEdinme");
            }

            var basvurular = _context.BilgiEdinmeBasvurulari
                .Where(b => (b.SahisTuru == "Sahis" && b.CepTelefonu == TakipCepTelefonu) ||
                           (b.SahisTuru == "Tuzel" && b.TuzelCepTelefonu == TakipCepTelefonu))
                .OrderByDescending(b => b.OlusturmaTarihi)
                .ToList();

            if (basvurular == null || !basvurular.Any())
            {
                TempData["TakipErrorMessage"] = "Bu cep telefonu numarası ile kayıtlı başvuru bulunamadı.";
                return RedirectToAction("BilgiEdinme");
            }

            var basvuruIds = string.Join(",", basvurular.Select(b => b.Id));
            HttpContext.Session.SetString("TakipBasvuruIds", basvuruIds);
            HttpContext.Session.SetString("TakipCepTelefonu", TakipCepTelefonu);
            
            TempData["TakipSuccessMessage"] = $"{basvurular.Count} adet başvuru bulundu.";
            TempData["BasvuruSayisi"] = basvurular.Count;

            return RedirectToAction("BilgiEdinme");
        }

        public IActionResult IstekVeOneri()
        {
            ViewData["PageTitle"] = "İstek ve Öneri";
            ViewData["ContentTitle"] = "İSTEK VE ÖNERİ";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "İstek ve Öneri", Url = (string?)null }
            };

            return View();
        }

        [HttpPost]
        public IActionResult IstekVeOneriGonder(string KimlikNo, int? DogumGunu, int? DogumAyi, string DogumYili,
            string Ad, string Soyad, string CepTelefonu, string Email, string BasvuruMetni, 
            bool KisiBilgilerimiGizle, string CaptchaCode)
        {
            if (string.IsNullOrEmpty(BasvuruMetni))
            {
                TempData["ErrorMessage"] = "Lütfen başvuru metnini doldurun.";
                return RedirectToAction("IstekVeOneri");
            }

            if (!KisiBilgilerimiGizle)
            {
                if (string.IsNullOrEmpty(KimlikNo) || !DogumGunu.HasValue || !DogumAyi.HasValue || 
                    string.IsNullOrEmpty(DogumYili) || string.IsNullOrEmpty(Ad) || 
                    string.IsNullOrEmpty(Soyad) || string.IsNullOrEmpty(CepTelefonu))
                {
                    TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doldurun.";
                    return RedirectToAction("IstekVeOneri");
                }
            }

            var sessionCaptcha = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(CaptchaCode) || string.IsNullOrEmpty(sessionCaptcha))
            {
                TempData["ErrorMessage"] = "Lütfen captcha kodunu girin.";
                return RedirectToAction("IstekVeOneri");
            }

            if (CaptchaCode.ToUpper() != sessionCaptcha.ToUpper())
            {
                TempData["ErrorMessage"] = "Captcha kodu hatalı. Lütfen tekrar deneyin.";
                HttpContext.Session.Remove("CaptchaCode");
                return RedirectToAction("IstekVeOneri");
            }

            HttpContext.Session.Remove("CaptchaCode");

            var istekVeOneri = new IstekVeOneri
            {
                KimlikNo = KisiBilgilerimiGizle ? null : KimlikNo,
                DogumGunu = KisiBilgilerimiGizle ? null : DogumGunu,
                DogumAyi = KisiBilgilerimiGizle ? null : DogumAyi,
                DogumYili = KisiBilgilerimiGizle ? null : DogumYili,
                Ad = KisiBilgilerimiGizle ? null : Ad,
                Soyad = KisiBilgilerimiGizle ? null : Soyad,
                CepTelefonu = KisiBilgilerimiGizle ? null : CepTelefonu,
                Email = KisiBilgilerimiGizle ? null : Email,
                BasvuruMetni = BasvuruMetni,
                KisiBilgilerimiGizle = KisiBilgilerimiGizle,
                OlusturmaTarihi = DateTime.Now,
                OkunduMu = false
            };

            try
            {
                _context.IstekVeOneriler.Add(istekVeOneri);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "İsteğiniz/öneriniz başarıyla alınmıştır. En kısa sürede size dönüş yapacağız.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İstek/öneri kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
                if (HttpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
                {
                    TempData["ErrorMessage"] += $" Hata: {ex.Message}";
                }
            }

            return RedirectToAction("IstekVeOneri");
        }

        public async Task<IActionResult> TelefonRehberi()
        {
            ViewData["PageTitle"] = "Telefon Rehberi";
            ViewData["ContentTitle"] = "TELEFON REHBERİ";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Telefon Rehberi", Url = (string?)null }
            };

            var telefonRehberi = await _context.TelefonRehberi
                .Where(t => t.AktifMi == true)
                .OrderBy(t => t.SiraNo)
                .ThenBy(t => t.KurumAdi)
                .ToListAsync();

            return View(telefonRehberi);
        }

        public async Task<IActionResult> YararliLinkler()
        {
            ViewData["PageTitle"] = "Yararlı Linkler";
            ViewData["ContentTitle"] = "YARARLI LİNKLER";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Yararlı Linkler", Url = (string?)null }
            };

            return View();
        }

        public async Task<IActionResult> UlusalLinkler()
        {
            ViewData["PageTitle"] = "Ulusal Linkler";
            ViewData["ContentTitle"] = "ULUSAL LİNKLER";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Yararlı Linkler", Url = (string?)(Url.Action("YararliLinkler", "Iletisim") ?? "#") },
                new { Text = "Ulusal Linkler", Url = (string?)null }
            };

            var ulusalLinkler = await _context.YararliLinkler
                .Where(l => l.AktifMi == true && l.Kategori == "Ulusal")
                .OrderBy(l => l.SiraNo)
                .ThenBy(l => l.LinkAdi)
                .ToListAsync();

            return View(ulusalLinkler);
        }

        public async Task<IActionResult> YerelLinkler()
        {
            ViewData["PageTitle"] = "Yerel Linkler";
            ViewData["ContentTitle"] = "YEREL LİNKLER";
            ViewBag.ShowBackButton = true;

            ViewBag.Breadcrumbs = new[]
            {
                new { Text = "Anasayfa", Url = (string?)(Url.Action("Index", "Home") ?? "#") },
                new { Text = "İletişim", Url = (string?)"#" },
                new { Text = "Yararlı Linkler", Url = (string?)(Url.Action("YararliLinkler", "Iletisim") ?? "#") },
                new { Text = "Yerel Linkler", Url = (string?)null }
            };

            var yerelLinkler = await _context.YararliLinkler
                .Where(l => l.AktifMi == true && l.Kategori == "Yerel")
                .OrderBy(l => l.SiraNo)
                .ThenBy(l => l.LinkAdi)
                .ToListAsync();

            return View(yerelLinkler);
        }
    }
}

