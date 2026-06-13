using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Duyuru modeli - Admin panel için
    /// </summary>
    public class Duyuru
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Aciklama { get; set; } = string.Empty;

        public bool YayindaMi { get; set; } = true;

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}






