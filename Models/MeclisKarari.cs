using System;
using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class MeclisKarari
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(500)]
        public string Baslik { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? KararNo { get; set; }
        
        [Required]
        public DateTime Tarih { get; set; }
        
        [StringLength(5000)]
        public string? Aciklama { get; set; }
        
        [StringLength(500)]
        public string? PdfUrl { get; set; }
        
        public bool AktifMi { get; set; } = true;
        
        public int SiraNo { get; set; }
    }
}

















