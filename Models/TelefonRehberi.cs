using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class TelefonRehberi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kurum Adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Kurum Adı en fazla 200 karakter olabilir.")]
        public string KurumAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur.")]
        [StringLength(100, ErrorMessage = "Telefon en fazla 100 karakter olabilir.")]
        public string Telefon { get; set; } = string.Empty;

        public int SiraNo { get; set; } = 0;

        public bool AktifMi { get; set; } = true;
    }
}

