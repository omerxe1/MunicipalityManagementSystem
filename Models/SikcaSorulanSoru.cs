using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Sıkça Sorulan Sorular modeli
    /// </summary>
    public class SikcaSorulanSoru
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Soru başlığı zorunludur.")]
        [StringLength(500, ErrorMessage = "Soru başlığı en fazla 500 karakter olabilir.")]
        [Column("SoruBaslik")]
        public string SoruBaslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cevap içeriği zorunludur.")]
        [Column("CevapIcerik", TypeName = "ntext")]
        public string CevapIcerik { get; set; } = string.Empty;

        [Column("SiraNo")]
        public int SiraNo { get; set; } = 0;

        [Column("AktifMi")]
        public bool AktifMi { get; set; } = true;

        [Column("OlusturmaTarihi")]
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        [Column("GuncellemeTarihi")]
        public DateTime? GuncellemeTarihi { get; set; }
    }
}



