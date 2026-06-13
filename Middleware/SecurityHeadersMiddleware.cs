namespace kocaaliv2.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // X-Frame-Options: Clickjacking koruması (sayfanın iframe içinde gösterilmesini engeller)
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // X-Content-Type-Options: MIME type sniffing koruması
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Referrer-Policy: Referrer bilgisinin gönderilmemesi
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");

            await _next(context);
        }
    }
}


