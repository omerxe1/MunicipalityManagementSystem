using Microsoft.EntityFrameworkCore;
using kocaaliv2.Data;
using kocaaliv2.Models;

namespace kocaaliv2.Services
{
    /// <summary>
    /// Login güvenlik servisi - Brute Force koruması
    /// 5 başarısız denemeden sonra 15 dakika hesap kilitleme
    /// </summary>
    public class LoginSecurityService : ILoginSecurityService
    {
        private readonly KocaaliContext _context;
        private const int MAX_FAILED_ATTEMPTS = 5;
        private const int LOCKOUT_DURATION_MINUTES = 15;

        public LoginSecurityService(KocaaliContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hesabın kilitli olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> IsAccountLockedAsync(string kullaniciAdi, string ipAdresi)
        {
            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.IPAdresi == ipAdresi);

            if (attempt == null)
                return false;

            // Eğer kilitlenme tarihi varsa ve süre dolmamışsa kilitli
            if (attempt.KilitlenmeTarihi.HasValue)
            {
                var lockoutEnd = attempt.KilitlenmeTarihi.Value.AddMinutes(LOCKOUT_DURATION_MINUTES);
                if (DateTime.Now < lockoutEnd)
                {
                    return true;
                }
                else
                {
                    // Kilit süresi dolmuş, kaydı temizle
                    _context.LoginAttempts.Remove(attempt);
                    await _context.SaveChangesAsync();
                    return false;
                }
            }

            // 5 veya daha fazla başarısız deneme varsa kilitle
            if (attempt.BasarisizDenemeSayisi >= MAX_FAILED_ATTEMPTS)
            {
                // İlk kez kilitleme yapılıyorsa tarih kaydet
                if (!attempt.KilitlenmeTarihi.HasValue)
                {
                    attempt.KilitlenmeTarihi = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Başarısız login denemesini kaydeder
        /// </summary>
        public async Task RecordFailedAttemptAsync(string kullaniciAdi, string ipAdresi)
        {
            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.IPAdresi == ipAdresi);

            if (attempt == null)
            {
                // Yeni kayıt oluştur
                attempt = new LoginAttempt
                {
                    KullaniciAdi = kullaniciAdi,
                    IPAdresi = ipAdresi,
                    BasarisizDenemeSayisi = 1,
                    SonDenemeTarihi = DateTime.Now
                };
                _context.LoginAttempts.Add(attempt);
            }
            else
            {
                // Mevcut kaydı güncelle
                attempt.BasarisizDenemeSayisi++;
                attempt.SonDenemeTarihi = DateTime.Now;

                // 5. başarısız denemede kilitle
                if (attempt.BasarisizDenemeSayisi >= MAX_FAILED_ATTEMPTS)
                {
                    attempt.KilitlenmeTarihi = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Başarılı login sonrası deneme kayıtlarını temizler
        /// </summary>
        public async Task ClearFailedAttemptsAsync(string kullaniciAdi, string ipAdresi)
        {
            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.IPAdresi == ipAdresi);

            if (attempt != null)
            {
                _context.LoginAttempts.Remove(attempt);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Kalan deneme hakkını döndürür
        /// </summary>
        public async Task<int> GetRemainingAttemptsAsync(string kullaniciAdi, string ipAdresi)
        {
            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(a => a.KullaniciAdi == kullaniciAdi && a.IPAdresi == ipAdresi);

            if (attempt == null)
                return MAX_FAILED_ATTEMPTS;

            return Math.Max(0, MAX_FAILED_ATTEMPTS - attempt.BasarisizDenemeSayisi);
        }
    }
}






