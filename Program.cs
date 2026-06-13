using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Services;
using kocaaliv2.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// FAZ 4: Session servisi ekle (güvenlik iyileştirmeleri)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15); // Admin panel için 15 dakika timeout
    options.Cookie.HttpOnly = true; // XSS koruması
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS için
    options.Cookie.SameSite = SameSiteMode.Strict; // CSRF koruması
});

// FAZ 4: Entity Framework Core servis kaydı
builder.Services.AddDbContext<kocaaliv2.Data.KocaaliContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KocaaliContext")));

// FAZ 4: Güvenlik servisleri
builder.Services.AddScoped<ILoginSecurityService, LoginSecurityService>();
builder.Services.AddScoped<IAdminLogService, AdminLogService>();
builder.Services.AddScoped<IReCaptchaService, ReCaptchaService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// FAZ 4: HTTP Client Factory (reCAPTCHA için)
builder.Services.AddHttpClient();


var app = builder.Build();

// FAZ 4: Content-Type ayarı
app.Use(async (context, next) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await next();
});

// FAZ 4: IP Kısıtlama Middleware (Admin Area için)
app.UseMiddleware<IPRestrictionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // FAZ 4: Production ortamı için özel error handling
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Development ortamında detaylı hata sayfası
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// FAZ 4: Session ve Authorization
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

// Gizli admin giriş URL'i - /yonetim-portal
app.MapControllerRoute(
    name: "adminLogin",
    pattern: "yonetim-portal",
    defaults: new { area = "Admin", controller = "Login", action = "Index" });

// Area routing - Admin panel için
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();