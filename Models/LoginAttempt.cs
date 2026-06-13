using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Brute Force koruması için başarısız login denemelerini takip eden model
    /// </summary>
    public class LoginAttempt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(45)]
        public string IPAdresi { get; set; } = string.Empty;

        public int BasarisizDenemeSayisi { get; set; } = 0;

        public DateTime? KilitlenmeTarihi { get; set; }

        public DateTime SonDenemeTarihi { get; set; } = DateTime.Now;
    }
}






