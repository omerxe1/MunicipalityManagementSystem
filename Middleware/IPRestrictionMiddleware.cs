using System.Net;

namespace kocaaliv2.Middleware
{
    /// <summary>
    /// IP kısıtlama middleware'i
    /// Admin paneline sadece izin verilen IP'lerden erişim sağlar
    /// </summary>
    public class IPRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IPRestrictionMiddleware> _logger;
        private readonly List<string> _allowedIPs;

        public IPRestrictionMiddleware(
            RequestDelegate next, 
            IConfiguration configuration,
            ILogger<IPRestrictionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            
            // appsettings.json'dan izin verilen IP'leri oku
            var allowedIPs = configuration.GetSection("Security:AllowedIPs").Get<List<string>>() ?? new List<string>();
            _allowedIPs = allowedIPs;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sadece Admin Area için IP kontrolü yap
            if (context.Request.Path.StartsWithSegments("/Admin") || 
                context.Request.Path.StartsWithSegments("/yonetim-portal"))
            {
                var clientIP = GetClientIP(context);

                // IP kısıtlaması aktifse ve IP listede yoksa erişimi engelle
                if (_allowedIPs.Any() && !_allowedIPs.Contains(clientIP) && !_allowedIPs.Contains("*"))
                {
                    _logger.LogWarning($"Yetkisiz IP erişim denemesi: {clientIP} - {context.Request.Path}");
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Bu IP adresinden erişim yetkiniz yok.");
                    return;
                }
            }

            await _next(context);
        }

        /// <summary>
        /// İstemci IP adresini alır (Proxy/Load Balancer desteği ile)
        /// </summary>
        private string GetClientIP(HttpContext context)
        {
            // X-Forwarded-For header'ından IP al (Load Balancer/Proxy için)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            // X-Real-IP header'ından IP al
            var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            // Direkt bağlantı IP'si
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}






