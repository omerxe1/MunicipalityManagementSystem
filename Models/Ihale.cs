using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Ihale
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Aciklama { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public DateTime Tarih { get; set; }

        [Required, StringLength(50)]
        public string Durum { get; set; } = string.Empty; // "Devam Eden", "Tamamlanan", "Planlanan"

        [StringLength(500)]
        public string? PdfUrl { get; set; }

        public DateTime OlusturmaTarihi { get; set; }
    }
}


