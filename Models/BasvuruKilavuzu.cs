using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class BasvuruKilavuzu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Aciklama { get; set; }

        [StringLength(500, ErrorMessage = "Dosya URL'si en fazla 500 karakter olabilir.")]
        public string? DosyaUrl { get; set; }

        [StringLength(500, ErrorMessage = "PDF URL'si en fazla 500 karakter olabilir.")]
        public string? PdfUrl { get; set; }

        [StringLength(500, ErrorMessage = "Dış link URL'si en fazla 500 karakter olabilir.")]
        public string? DisLink { get; set; }

        public int Sira { get; set; } = 0;

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}

