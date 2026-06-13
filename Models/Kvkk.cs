using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Kvkk
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Aciklama { get; set; }

        [StringLength(500)]
        public string? PdfUrl { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public bool AktifMi { get; set; } = true;

        public int SiraNo { get; set; }
    }
}


