using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace kocaaliv2.Filters
{
    /// <summary>
    /// Admin Area içindeki sayfalara erişimi kontrol eden ActionFilter
    /// Giriş yapılmamışsa Login sayfasına yönlendirir
    /// </summary>
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Session'dan kullanıcı bilgilerini kontrol et
            var kullaniciAdi = context.HttpContext.Session.GetString("AdminKullaniciAdi");
            var rol = context.HttpContext.Session.GetString("AdminRol");

            // Eğer session'da kullanıcı bilgisi yoksa Login sayfasına yönlendir
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(rol))
            {
                context.Result = new RedirectToActionResult("Index", "Login", new { area = "Admin" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}






