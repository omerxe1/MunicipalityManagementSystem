using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kocaaliv2.Models
{
    public class Etkinlik
    {
        [Key]
        [Column("EtkinlikID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Column("EtkinlikAdi")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Column("EtkinlikAciklama")]
        public string Aciklama { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir.")]
        [Column("EtkinlikResimYolu")]
        public string? ResimYolu { get; set; }

        [Column("EtkinlikTarihi")]
        public DateTime Tarih { get; set; }

        [StringLength(100, ErrorMessage = "Kategori en fazla 100 karakter olabilir.")]
        [Column("EtkinlikKategori")]
        public string? Kategori { get; set; }

        [StringLength(200, ErrorMessage = "Mekan en fazla 200 karakter olabilir.")]
        [Column("EtkinlikMekan")]
        public string? Mekan { get; set; }

        public bool YayindaMi { get; set; } = true;

        [Column("BaslangicTarihi")]
        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;

        [Column("BitisTarihi")]
        public DateTime? BitisTarihi { get; set; }
    }
}

