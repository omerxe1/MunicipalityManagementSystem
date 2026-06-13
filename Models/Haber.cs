using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Haber modeli - Admin panel için
    /// </summary>
    public class Haber
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(150, ErrorMessage = "Başlık en fazla 150 karakter olabilir")]
        public string Baslik { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "İçerik zorunludur")]
        public string Icerik { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? ResimYolu { get; set; }
        
        public bool YayindaMi { get; set; } = true;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
