using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class AfadAcilToplanmaAlani
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(200)]
        public string Adi { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string Ilce { get; set; } = string.Empty;
        
        [Required, StringLength(200)]
        public string Mahalle { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Enlem { get; set; }
        
        [StringLength(50)]
        public string? Boylam { get; set; }
        
        public bool Elektrik { get; set; }
        
        public bool Yol { get; set; }
        
        public bool WcKanSistemi { get; set; }
        
        public int SiraNo { get; set; }
        
        public bool AktifMi { get; set; } = true;
    }
}














