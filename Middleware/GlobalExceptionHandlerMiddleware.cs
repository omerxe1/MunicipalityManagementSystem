using System.Net;
using kocaaliv2.Services;

namespace kocaaliv2.Middleware
{
    
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // FAZ 4: Hatayı logla
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var clientIP = GetClientIP(context);
            var path = context.Request.Path;
            var userName = context.User?.Identity?.Name ?? 
                          context.Session?.GetString("AdminKullaniciAdi");
            var securityLogService = context.RequestServices.GetService<SecurityLogService>();
            securityLogService?.LogCriticalError(
                $"Unhandled exception - Path: {path}",
                exception,
                clientIP,
                userName);

            _logger.LogError(exception, 
                "FAZ 4: Unhandled exception - IP: {IP}, Path: {Path}, User: {User}",
                clientIP, path, userName);

            throw exception;
        }

        private string GetClientIP(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}

