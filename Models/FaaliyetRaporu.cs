using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class FaaliyetRaporu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Yil { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public bool AktifMi { get; set; } = true;
    }
}


