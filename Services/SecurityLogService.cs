using Microsoft.Extensions.Logging;

namespace kocaaliv2.Services
{
    /// <summary>
    /// FAZ 4: Güvenlik olaylarını loglayan servis
    /// Hata, güvenlik ihlalleri ve şüpheli aktiviteleri loglar
    /// </summary>
    public class SecurityLogService
    {
        private readonly ILogger<SecurityLogService> _logger;

        public SecurityLogService(ILogger<SecurityLogService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// FAZ 4: Güvenlik olayını loglar
        /// </summary>
        public void LogSecurityEvent(string eventType, string message, string? ipAddress = null, string? userName = null, Exception? exception = null)
        {
            var logMessage = $"[SECURITY] {eventType} - {message}";
            
            if (!string.IsNullOrEmpty(ipAddress))
            {
                logMessage += $" | IP: {ipAddress}";
            }
            
            if (!string.IsNullOrEmpty(userName))
            {
                logMessage += $" | User: {userName}";
            }

            if (exception != null)
            {
                _logger.LogError(exception, logMessage);
            }
            else
            {
                _logger.LogWarning(logMessage);
            }
        }

        /// <summary>
        /// FAZ 4: Brute force saldırısı loglar
        /// </summary>
        public void LogBruteForceAttempt(string ipAddress, string? userName = null)
        {
            LogSecurityEvent("BRUTE_FORCE", 
                $"Brute force saldırı tespit edildi", 
                ipAddress, 
                userName);
        }

        /// <summary>
        /// FAZ 4: Rate limit aşımı loglar
        /// </summary>
        public void LogRateLimitExceeded(string ipAddress, string path)
        {
            LogSecurityEvent("RATE_LIMIT", 
                $"Rate limit aşıldı - Path: {path}", 
                ipAddress);
        }

        /// <summary>
        /// FAZ 4: Yetkisiz erişim denemesi loglar
        /// </summary>
        public void LogUnauthorizedAccess(string ipAddress, string path, string? userName = null)
        {
            LogSecurityEvent("UNAUTHORIZED_ACCESS", 
                $"Yetkisiz erişim denemesi - Path: {path}", 
                ipAddress, 
                userName);
        }

        /// <summary>
        /// FAZ 4: Şüpheli aktivite loglar
        /// </summary>
        public void LogSuspiciousActivity(string activity, string ipAddress, string? userName = null)
        {
            LogSecurityEvent("SUSPICIOUS_ACTIVITY", 
                $"Şüpheli aktivite: {activity}", 
                ipAddress, 
                userName);
        }

        /// <summary>
        /// FAZ 4: Kritik hata loglar
        /// </summary>
        public void LogCriticalError(string message, Exception exception, string? ipAddress = null, string? userName = null)
        {
            LogSecurityEvent("CRITICAL_ERROR", 
                message, 
                ipAddress, 
                userName, 
                exception);
        }
    }
}



