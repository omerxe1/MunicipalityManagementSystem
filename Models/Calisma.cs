using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Calisma
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir.")]
        public string? ImageUrl { get; set; }

        public DateTime PublishDate { get; set; }

        [StringLength(50)]
        public string? Durum { get; set; } // "Devam Eden", "Tamamlanan", "Planlanan"
    }
}


















