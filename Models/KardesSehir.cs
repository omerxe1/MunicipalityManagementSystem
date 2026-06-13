using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class KardesSehir
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string BelediyeAdi { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string SehirAdi { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Ulke { get; set; }

        [StringLength(50)]
        public string? KararNo { get; set; }

        [StringLength(500)]
        public string? ResimYolu { get; set; }

        [StringLength(1000)]

        public DateTime? ProtokolTarihi { get; set; }

        public int SiraNo { get; set; }

        public bool AktifMi { get; set; } = true;
    }
}

