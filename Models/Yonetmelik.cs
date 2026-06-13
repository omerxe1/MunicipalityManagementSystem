using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Yonetmelik
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(200)]
        public string Baslik { get; set; } = string.Empty;
        
        [Required, StringLength(500)]
        public string PdfUrl { get; set; } = string.Empty;
        
        public bool AktifMi { get; set; } = true;
        
        public int SiraNo { get; set; }
    }
}



















