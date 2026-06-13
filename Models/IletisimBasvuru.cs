using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    /// <summary>
    /// İletişim başvuru modeli - Admin panel için
    /// </summary>
    public class IletisimBasvuru
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(200, ErrorMessage = "Email en fazla 200 karakter olabilir")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konu zorunludur")]
        [StringLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir")]
        public string Konu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [StringLength(2000, ErrorMessage = "Mesaj en fazla 2000 karakter olabilir")]
        public string Mesaj { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;
    }
}






