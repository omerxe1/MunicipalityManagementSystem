using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Admin kullanıcıları için model sınıfı
    /// </summary>
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string SifreHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "Editor"; // Admin veya Editor

        public bool AktifMi { get; set; } = true;
    }
}






