# Güvenlik Denetim Raporu – Kocaali Belediyesi Web Sitesi

**Tarih:** Ocak 2025  
**Kapsam:** HTTPS, Admin yetkilendirme, Dosya upload, PDF/linkler, Loglama, SQL injection, XSS, Yedekleme  
**Sınıflandırma:** Kritik / Orta / Düşük – Canlıya çıkmadan MUTLAKA düzeltilmesi gerekenler işaretlidir.

---

## 1️⃣ HTTPS (SSL)

### ✅ Mevcut durum (iyi)
- **Dosya:** `Program.cs`  
- **Satır:** 67, 69–72  
- HTTP → HTTPS yönlendirmesi: `app.UseHttpsRedirection()` ve Production’da `app.UseHsts()` kullanılıyor.
- Cookie: Session için `HttpOnly = true`, `SecurePolicy`: Production’da `CookieSecurePolicy.Always`, `SameSite = Strict`.

### ⚠️ Eksik / risk
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| Proje kökü | **web.config yok.** IIS ile host edilirse HTTPS redirect ve request limitleri web.config’te de tanımlanmalı. | IIS kullanılıyorsa `web.config` ekleyin: `<system.webServer>` içinde `rewrite` ile HTTP→HTTPS ve gerekirse `security` / `requestFiltering`. |
| View’lar (Layout, Index, vb.) | **Mixed Content:** Tüm harici kaynaklar `https://` kullanıyor (CDN, iframe, img). Tek risk: Faaliyet Raporu/Admin’de **http://** kabul eden URL validasyonu. | Harici link/PDF alanlarında yalnızca `https://` kabul edin; `http://` kabul etmeyin (özellikle kullanıcı/admin girdisi ise). |

**Özet:** Kritik bir HTTPS açığı yok. IIS kullanımında web.config ve sadece HTTPS link kabulü eklenmeli (orta/düşük).

---

## 2️⃣ Admin Paneli Yetkilendirme

### ✅ İyi
- Tüm Admin controller’lar (Login hariç) **`[AdminAuthorize]`** ile korunuyor.
- `AdminAuthorizeAttribute`: Session’da `AdminKullaniciAdi` ve `AdminRol` yoksa Login’e yönlendiriyor.
- Login sayfası `[AdminAuthorize]` olmadan; Logout, Dashboard ve diğer aksiyonlar korumalı.

### 🔴 KRİTİK – Canlıya çıkmadan düzeltin
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **Controllers/SeedController.cs** (tüm sınıf) | **`[Authorize]` veya `[AllowAnonymous]` yok.** Herkes `/Seed/CreateAdmin` ve `/Seed/CreateBasAdmin` çağırabilir; admin/basadmin hesabı oluşturulur (şifre: admin123 / basadmin123). | **Seçenek 1:** Production’da Seed controller’ı tamamen devre dışı bırakın (route kaldırın veya `if (!env.IsDevelopment()) return NotFound();`). **Seçenek 2:** Sadece Development’ta çalışsın; Production’da `return NotFound()`. |

**Örnek düzeltme (SeedController):**

```csharp
// SeedController.cs - sınıfın başına
public class SeedController : Controller
{
    private readonly KocaaliContext _context;
    private readonly IWebHostEnvironment _env;

    public SeedController(KocaaliContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult CreateAdmin()
    {
        if (!_env.IsDevelopment())
            return NotFound();
        // ... mevcut kod
    }

    public IActionResult CreateBasAdmin()
    {
        if (!_env.IsDevelopment())
            return NotFound();
        // ... mevcut kod
    }
}
```

### Orta risk
- Admin yetkisi **sadece controller bazlı** `[AdminAuthorize]`. Yeni eklenen bir Admin controller’da attribute unutulursa sayfa açık kalır.
- **Öneri:** Admin Area için global filter ekleyin; sadece `Login`, `SifremiUnuttum` gibi aksiyonları `[AllowAnonymous]` ile hariç tutun.

---

## 3️⃣ Dosya Upload Güvenliği & Limitleri

### ✅ İyi
- **Services/FileUploadService.cs:** Uzantı whitelist (`.pdf`, `.jpg`, `.jpeg`, `.png`), MIME whitelist, boyut limiti (varsayılan 5MB), güvenli dosya adı (random + timestamp), **wwwroot dışına çıkış engeli** (`Path.GetFullPath` + `StartsWith(webRoot)`).
- Upload ve silme yolları wwwroot içinde normalize edilip kontrol ediliyor.

### ⚠️ Orta risk
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **FileUploadService.cs** satır 53–55 | MIME kontrolü sadece **`file.ContentType`** ile. İstekte Content-Type sahtelenebilir; tehlikeli dosya PDF/PNG gibi kabul edilebilir. | Dosya imzası (magic bytes) kontrolü ekleyin. Örn. ilk birkaç byte’ı okuyup PDF (örn. `%PDF`), JPEG (`FF D8`), PNG (`89 50 4E`) ile eşleştirin. |
| **Program.cs / Startup** | **MultipartBodyLengthLimit** veya Kestrel **MaxRequestBodySize** ayarı yok. Varsayılan limit (28MB civarı) büyük; büyük upload ile DoS riski. | `builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 10 * 1024 * 1024);` (10MB) ve gerekirse Kestrel’de `MaxRequestBodySize` sınırlayın. |

### Düşük / bilgi
- **web.config yok:** Proje ASP.NET Core, Kestrel ile çalışıyor. IIS kullanılıyorsa `maxRequestLength` ve `requestFiltering` için web.config eklenmeli.
- Upload’lar wwwroot altında; çalıştırılabilir uzantı (.aspx, .exe, .dll vb.) whitelist’te yok – doğru.

---

## 4️⃣ PDF / Duyuru / Haber Linkleri

### ✅ İyi
- Faaliyet raporu / PDF URL’leri veritabanından geliyor; admin harici URL girebiliyor (http/https). View’da `StartsWith("http://") || StartsWith("https://")` ile link gösterimi var.
- Kullanıcıdan doğrudan path parametresi alan bir “dosya indir” endpoint’i yok; KvkkPdfGoruntule/Indir **id** ile çalışıyor.

### 🔴 KRİTİK – Path traversal (dosya yolu)
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **Controllers/KurumsalController.cs** satır 329, 353 | `filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dokuman.PdfUrl.TrimStart('/'));` – **Path.GetFullPath** ve **wwwroot içinde mi** kontrolü yok. Veritabanında `PdfUrl` (örn. SQL/Admin ihlali) `../` içeriyorsa wwwroot dışına okuma riski. | Önce `Path.GetFullPath` ile normalize edin, sonra `fullPath.StartsWith(Path.GetFullPath(WebRootPath))` ile sınırlayın. |

**Örnek düzeltme (KurumsalController.cs):**

```csharp
// KvkkPdfGoruntule ve KvkkPdfIndir içinde
var webRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
var fullPath = Path.GetFullPath(Path.Combine(webRoot, dokuman.PdfUrl.TrimStart('/').Replace('\\', '/')));
if (!fullPath.StartsWith(webRoot) || !System.IO.File.Exists(fullPath))
    return NotFound();
var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
// ...
```

### Orta risk
- Hard-coded harici linkler (eczane iframe, YouTube, Facebook, placeholder görseller) View’larda var; kırık link estetik/servis sorunudur, güvenlik açığı değil. Mümkünse yönetilebilir (config/DB) yapın.

---

## 5️⃣ Loglama (Try-Catch & Hata Yönetimi)

### ✅ İyi
- **GlobalExceptionHandlerMiddleware:** Yakalanmamış exception’ları logluyor (SecurityLogService + ILogger), sonra exception’ı yeniden fırlatıyor; Production’da **UseExceptionHandler("/Home/Error")** kullanıcıyı hata sayfasına götürüyor.
- **HomeController.Error:** Sadece statusCode ve genel mesaj; exception/stack trace kullanıcıya verilmiyor.
- **Error.cshtml:** Sadece `RequestId` ve “Development’ta daha fazla bilgi” metni; Production’da stack trace yok.

### ⚠️ Orta risk
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **GlobalExceptionHandlerMiddleware.cs** satır 50 | Exception loglandıktan sonra **`throw exception;`** – doğru. Ancak **InvokeAsync** içinde `await _next(context)` try-catch’te; `HandleExceptionAsync` **async** ama **await** kullanmıyor (uyarı CS1998). | `HandleExceptionAsync` zaten senkron iş + Log + throw yapıyorsa mevcut yapı çalışır; uyarıyı gidermek için `await Task.CompletedTask;` veya metodu senkron yapıp çağıranı `HandleExceptionAsync(context, ex).GetAwaiter().GetResult()` yapmak yerine, sadece `await HandleExceptionAsync(context, ex);` kullanın ve gerekirse içeride `return;` sonrası throw edin. |
| **Loglar nereye?** | Loglar şu an **ILogger** ile; varsayılanla genelde konsol/debug. Production’da kalıcı ve rotasyonlu dosya (veya veritabanı) önerilir. | `appsettings.Production.json` içinde Serilog/NLog vb. ile dosya sink’i (örn. `logs/`) ve log seviyesi (Warning/Error) ayarlayın. |

### Düşük
- Bazı controller’larda (örn. AdminController) catch içinde **`ModelState.AddModelError("", ex.Message);`** – kullanıcıya hata mesajı gidiyor; hassas detay (path, DB bilgisi) içermemeli. Genel mesaj kullanın.

---

## 6️⃣ SQL Injection Kontrolü

### ✅ Sonuç: Risk tespit edilmedi
- **FromSqlRaw / ExecuteSqlRaw / string concatenation ile sorgu** kullanımı yok.
- Veri erişimi **Entity Framework Core** (LINQ / DbSet) ile yapılıyor; parametreler EF tarafından yönetiliyor.

**Öneri:** İleride raw SQL kullanılırsa mutlaka parametreli API kullanın (`FromSqlInterpolated` / `SqlParameter`).

---

## 7️⃣ XSS (Cross-Site Scripting)

### 🔴 KRİTİK – Canlıya çıkmadan düzeltin
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **Views/Haberler/Detay.cshtml** satır 30 | **`@Html.Raw(Model.Icerik)`** – Haber içeriği veritabanından geliyor; admin veya veri ihlali ile `<script>`, `<iframe>` vb. eklenirse XSS çalışır. | İçeriği **HTML olarak saklıyorsanız** bile çıktıda güvenli hale getirin: bir “allowed tag” listesi ile (ör. `<p>, <br>, <strong>, <a>`) sadece izin verilen etiketlere izin veren bir kütüphane kullanın (örn. HtmlSanitizer NuGet). Veya rich text yerine düz metin + satır sonu→`<br>` kullanın; o zaman `@Html.Raw` kullanmayın, encode edin. |

**Örnek (HtmlSanitizer):**

```csharp
// Controller'da veya view'da (helper ile)
using HtmlSanitizer;
var sanitizer = new HtmlSanitizer();
Model.Icerik = sanitizer.Sanitize(Model.Icerik);
```

```html
@Html.Raw(Model.Icerik)  <!-- Artık sanitize edilmiş -->
```

### Orta risk
| Nerede | Neden riskli | Nasıl düzeltilir |
|--------|--------------|------------------|
| **Views/Shared/_Layout.cshtml** satır 910 | **`popupAnnouncementsData = @Html.Raw(popupDataJson);`** – Veri sunucudan JSON; Razor serialize ediyor. Eğer `Title` vb. alanlara script eklenirse JSON içinde break out riski. | JSON zaten `System.Text.Json` ile serialize ediliyor; property değerleri escape edilir. Risk düşük; yine de admin panelinde popup başlık/içerik alanlarına script girişi engellenmeli (input validasyonu veya sanitize). |
| **Views/Shared/_CalismalarPartial.cshtml** | **`@Html.Raw(Json.Serialize(Model))`** – Model sunucudan; EF’ten gelen veri. Admin “Çalışma” başlık/açıklama alanına script koyarsa bu JSON’a girer, sonra JS’te kullanılırsa XSS olabilir. | Çalışma verisi admin kaynaklı; admin panelinde HTML/script girişini kısıtlayın veya çıktıda encode/sanitize edin. |

### Zaten güvenli
- **MeclisKararlari.cshtml, ImarPlanlariDetay, IhaleDetay, DuyuruDetay, SikcaSorulanSorular:** `@Html.Raw(Html.Encode(...).Replace("\n", "<br>"))` – Önce encode, sonra sadece satır sonu `<br>`; XSS riski düşük.

---

## 8️⃣ Yedekleme (DB + Upload Klasörleri)

### Mevcut durum
- Kod tabanında **otomatik DB yedekleme** veya **upload klasörü yedekleme** betiği yok.

### Öneriler (belediye prod standardı)

| Konu | Öneri |
|------|--------|
| **Veritabanı** | SQL Server için: (1) Günlük tam yedek (örn. gece 02:00), (2) Haftalık tam + günlük diff/transaction log. Yedekler uygulama sunucusu dışında (ayrı disk/network) saklansın. |
| **Upload klasörleri** | `wwwroot/uploads`, `wwwroot/images` vb. için günlük/haftalık kopyalama (robocopy/xcopy veya backup yazılımı). Silinme/bozulmaya karşı kısa saklama süresi (örn. 7–30 gün). |
| **Geri dönüş** | (1) DB: Son bilinen iyi yedekten restore + transaction log restore. (2) Dosyalar: Son yedekten kopyala. Dokümante edin ve yılda en az bir kez test edin. |

Bu maddeler **uygulama kodu değişikliği gerektirmez**; altyapı/operasyon talimatı olarak eklenmeli.

---

## Özet Tablo

| Başlık | Kritik | Orta | Düşük | Canlıya çıkmadan mutlaka |
|--------|--------|------|-------|---------------------------|
| 1 HTTPS | – | web.config / sadece HTTPS link | – | Hayır |
| 2 Admin | SeedController | Global Admin filter | – | **Evet (Seed)** |
| 3 Upload | – | MIME magic bytes, body size limit | web.config IIS | Hayır |
| 4 PDF/Link | Path traversal (KurumsalController) | Hard-coded linkler | – | **Evet (path traversal)** |
| 5 Loglama | – | Async uyarı, log hedefi | ex.Message kullanımı | Hayır |
| 6 SQL Injection | – | – | – | – |
| 7 XSS | Haberler/Detay @Html.Raw(Icerik) | Popup/Calismalar JSON | – | **Evet (Haber detay)** |
| 8 Yedekleme | – | – | Dokümantasyon | Hayır |

---

## Canlıya çıkmadan MUTLAKA yapılacaklar

**✅ TAMAMLANDI (Ocak 2025):**

1. **SeedController** – `CreateAdmin` ve `CreateBasAdmin` artık yalnızca `IsDevelopment()` ise çalışıyor; Production’da `NotFound()` dönüyor. `IWebHostEnvironment` enjekte edildi.
2. **KurumsalController** – `KvkkPdfGoruntule` ve `KvkkPdfIndir` içinde `Path.GetFullPath` + wwwroot altında mı kontrolü eklendi; path traversal engellendi. `IWebHostEnvironment` enjekte edildi.
3. **Haberler/Detay** – NuGet paketi `HtmlSanitizer` eklendi; `HaberlerController.Detay` içinde haber içeriği `HtmlSanitizer.Sanitize()` ile temizlenip `ViewBag.SafeIcerik` ile view’a veriliyor; `Detay.cshtml` bu değeri kullanıyor (XSS riski azaltıldı).

Bu üç madde uygulandı; belediyeye uygun prod güvenlik standardı açısından kritik açıklar kapatıldı.
