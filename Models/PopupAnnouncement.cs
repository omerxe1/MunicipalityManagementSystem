using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class PopupAnnouncement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
        public string? ImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir.")]
        public string? Icon { get; set; } = "fas fa-bullhorn";

        public int SiraNo { get; set; } = 0;

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}









