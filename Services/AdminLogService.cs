using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Services
{
    /// <summary>
    /// Admin log servisi - Tüm işlemleri loglar
    /// </summary>
    public class AdminLogService : IAdminLogService
    {
        private readonly KocaaliContext _context;

        public AdminLogService(KocaaliContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Log kaydı oluşturur
        /// </summary>
        public async Task LogAsync(string kullaniciAdi, string islem, string? tabloAdi, string? aciklama, string ipAdresi)
        {
            try
            {
                var log = new AdminLog
                {
                    KullaniciAdi = kullaniciAdi,
                    Islem = islem,
                    TabloAdi = tabloAdi,
                    Aciklama = aciklama,
                    IPAdresi = ipAdresi,
                    Tarih = DateTime.Now
                };

                _context.AdminLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Log kaydı başarısız olsa bile uygulama çalışmaya devam etmeli
                // Sessizce hata yutulur
            }
        }
    }
}






