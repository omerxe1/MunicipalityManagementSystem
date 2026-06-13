using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Görsel URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Yükleme Tarihi")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}



