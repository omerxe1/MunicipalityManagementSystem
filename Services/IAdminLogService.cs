namespace kocaaliv2.Services
{
    /// <summary>
    /// Admin log işlemleri için servis interface'i
    /// </summary>
    public interface IAdminLogService
    {
        /// <summary>
        /// Log kaydı oluşturur
        /// </summary>
        Task LogAsync(string kullaniciAdi, string islem, string? tabloAdi, string? aciklama, string ipAdresi);
    }
}






