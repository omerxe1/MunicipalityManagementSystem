using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Admin panel işlemlerini loglamak için model
    /// </summary>
    public class AdminLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Islem { get; set; } = string.Empty; // Giriş, Çıkış, Ekleme, Güncelleme, Silme

        [StringLength(100)]
        public string? TabloAdi { get; set; }

        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Required]
        [StringLength(45)]
        public string IPAdresi { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}






