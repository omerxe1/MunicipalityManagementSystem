namespace kocaaliv2.Services
{
    /// <summary>
    /// Login güvenlik işlemleri için servis interface'i
    /// Brute Force koruması ve login denemelerini yönetir
    /// </summary>
    public interface ILoginSecurityService
    {
        /// <summary>
        /// Kullanıcı adı ve IP için başarısız deneme sayısını kontrol eder
        /// </summary>
        Task<bool> IsAccountLockedAsync(string kullaniciAdi, string ipAdresi);

        /// <summary>
        /// Başarısız login denemesini kaydeder
        /// </summary>
        Task RecordFailedAttemptAsync(string kullaniciAdi, string ipAdresi);

        /// <summary>
        /// Başarılı login sonrası deneme kayıtlarını temizler
        /// </summary>
        Task ClearFailedAttemptsAsync(string kullaniciAdi, string ipAdresi);

        /// <summary>
        /// Kalan deneme hakkını döndürür
        /// </summary>
        Task<int> GetRemainingAttemptsAsync(string kullaniciAdi, string ipAdresi);
    }
}






