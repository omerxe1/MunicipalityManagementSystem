using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class StratejikPlan
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Baslik { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Url { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public bool AktifMi { get; set; } = true;

        public int SiraNo { get; set; }
    }
}


