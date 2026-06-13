using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class QuickMenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "URL en fazla 500 karakter olabilir.")]
        public string? Url { get; set; }

        [StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir.")]
        public string? Icon { get; set; }

        [Required(ErrorMessage = "Menü kategorisi zorunludur.")]
        [StringLength(50, ErrorMessage = "Menü kategorisi en fazla 50 karakter olabilir.")]
        public string MenuKategori { get; set; } = string.Empty; // KOCAALİ, BELEDİYE_REHBERİ, E_BELEDİYE, E_PORTAL, HIZLI_MENÜ

        [StringLength(100, ErrorMessage = "Controller adı en fazla 100 karakter olabilir.")]
        public string? Controller { get; set; }

        [StringLength(100, ErrorMessage = "Action adı en fazla 100 karakter olabilir.")]
        public string? Action { get; set; }

        public int SiraNo { get; set; } = 0;

        public bool AktifMi { get; set; } = true;

        public bool YeniSekmedeAc { get; set; } = false;
    }
}









