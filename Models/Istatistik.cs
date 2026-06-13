using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kocaaliv2.Models
{
    /// <summary>
    /// İstatistik modeli - Ana sayfadaki istatistik kartları için
    /// </summary>
    public class Istatistik
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        [Column("Baslik")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Değer zorunludur.")]
        [StringLength(50, ErrorMessage = "Değer en fazla 50 karakter olabilir.")]
        [Column("Deger")]
        public string Deger { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "İkon sınıfı en fazla 100 karakter olabilir.")]
        [Column("Ikon")]
        public string? Ikon { get; set; } = "fas fa-chart-bar";

        [Column("SiraNo")]
        public int SiraNo { get; set; } = 0;

        [Column("AktifMi")]
        public bool AktifMi { get; set; } = true;

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}



