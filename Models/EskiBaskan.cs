using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class EskiBaskan
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(200)]
        public string AdSoyad { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string Donem { get; set; } = string.Empty;
        
        [Required, StringLength(500)]
        public string ResimYolu { get; set; } = string.Empty;
        
        public int SiraNo { get; set; }
        
        public bool AktifMi { get; set; } = true;
    }
}



















