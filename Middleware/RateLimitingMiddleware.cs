using System.Collections.Concurrent;
using System.Net;
using kocaaliv2.Services;

namespace kocaaliv2.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _requestCounts = new();
        
        private readonly int _maxRequestsPerMinute;
        private readonly int _maxRequestsPerHour;
        private readonly TimeSpan _windowSize = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _hourWindowSize = TimeSpan.FromHours(1);

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _maxRequestsPerMinute = configuration.GetValue<int>("Security:RateLimit:MaxRequestsPerMinute", 60);
            _maxRequestsPerHour = configuration.GetValue<int>("Security:RateLimit:MaxRequestsPerHour", 1000);
        }

        public async Task InvokeAsync(HttpContext context, SecurityLogService? securityLogService = null)
        {

            var clientIP = GetClientIP(context);
            
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (IsSuspiciousUserAgent(userAgent))
            {
                _logger.LogWarning($"FAZ 4: Şüpheli User-Agent tespit edildi - IP: {clientIP}, User-Agent: {userAgent}");
                securityLogService?.LogSuspiciousActivity($"Şüpheli User-Agent: {userAgent}", clientIP);
                
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Erişim reddedildi.");
                return;
            }
            
            if (!IsAllowed(clientIP, context.Request.Path))
            {
                _logger.LogWarning($"FAZ 4: Rate limit aşıldı - IP: {clientIP}, Path: {context.Request.Path}");

                securityLogService?.LogRateLimitExceeded(clientIP, context.Request.Path);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = "60"; // FAZ 4: 60 saniye sonra tekrar dene
                await context.Response.WriteAsync("Çok fazla istek gönderdiniz. Lütfen bir süre sonra tekrar deneyin.");
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// FAZ 4: IP'nin istek göndermesine izin verilip verilmeyeceğini kontrol eder
        /// </summary>
        private bool IsAllowed(string ipAddress, PathString path)
        {
            // FAZ 4: Admin panel ve login sayfaları için daha sıkı limit
            var isAdminPath = path.StartsWithSegments("/Admin") || 
                             path.StartsWithSegments("/yonetim-portal");
            
            var maxRequests = isAdminPath ? _maxRequestsPerMinute / 2 : _maxRequestsPerMinute;
            
            var now = DateTime.UtcNow;
            var key = ipAddress;

            // FAZ 4: Mevcut kaydı al veya yeni oluştur
            var rateLimitInfo = _requestCounts.GetOrAdd(key, _ => new RateLimitInfo());

            // FAZ 4: Eski kayıtları temizle (1 dakikadan eski)
            lock (rateLimitInfo)
            {
                rateLimitInfo.Requests.RemoveAll(r => (now - r).TotalMinutes > 1);
                rateLimitInfo.HourlyRequests.RemoveAll(r => (now - r).TotalHours > 1);

                // FAZ 4: Yeni isteği kaydet
                rateLimitInfo.Requests.Add(now);
                rateLimitInfo.HourlyRequests.Add(now);

                // FAZ 4: Dakika bazlı kontrol
                if (rateLimitInfo.Requests.Count > maxRequests)
                {
                    return false;
                }

                // FAZ 4: Saat bazlı kontrol
                if (rateLimitInfo.HourlyRequests.Count > _maxRequestsPerHour)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// FAZ 4: İstemci IP adresini alır (Proxy/Load Balancer desteği ile)
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

        /// <summary>
        /// FAZ 4: Şüpheli User-Agent kontrolü (Bot koruması)
        /// İyi bilinen arama motoru bot'larına izin verir, şüpheli bot'ları engeller
        /// </summary>
        private bool IsSuspiciousUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return true; // User-Agent yoksa şüpheli
            }

            var lowerUserAgent = userAgent.ToLowerInvariant();

            // İyi bilinen arama motoru bot'larına izin ver
            var allowedBots = new[]
            {
                "googlebot",
                "bingbot",
                "slurp", // Yahoo
                "duckduckbot",
                "baiduspider",
                "yandexbot",
                "sogou",
                "exabot",
                "facebot",
                "ia_archiver" // Internet Archive
            };

            if (allowedBots.Any(bot => lowerUserAgent.Contains(bot)))
            {
                return false; // İyi bilinen bot, şüpheli değil
            }

            // Şüpheli bot pattern'leri
            var suspiciousPatterns = new[]
            {
                "bot", "crawler", "spider", "scraper",
                "curl", "wget", "python-requests",
                "masscan", "nmap", "sqlmap"
            };

            return suspiciousPatterns.Any(pattern => lowerUserAgent.Contains(pattern));
        }

        /// <summary>
        /// FAZ 4: Rate limit bilgilerini tutan sınıf
        /// </summary>
        private class RateLimitInfo
        {
            public List<DateTime> Requests { get; } = new();
            public List<DateTime> HourlyRequests { get; } = new();
        }
    }
}

