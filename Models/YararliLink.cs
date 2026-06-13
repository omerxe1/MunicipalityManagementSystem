using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class YararliLink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Link Adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Link Adı en fazla 200 karakter olabilir.")]
        public string LinkAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL zorunludur.")]
        [StringLength(500, ErrorMessage = "URL en fazla 500 karakter olabilir.")]
        [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori en fazla 50 karakter olabilir.")]
        public string Kategori { get; set; } = string.Empty; // "Ulusal" veya "Yerel"

        [StringLength(500, ErrorMessage = "Logo URL en fazla 500 karakter olabilir.")]
        public string? LogoUrl { get; set; }

        public int SiraNo { get; set; } = 0;

        public bool AktifMi { get; set; } = true;
    }
}

