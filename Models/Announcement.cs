using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        [StringLength(1000, ErrorMessage = "İçerik en fazla 1000 karakter olabilir.")]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
    }
}
