using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class BilgiEdinmeBasvuru
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string SahisTuru { get; set; } = string.Empty; // "Sahis" veya "Tuzel"

        // Şahıs için alanlar
        [StringLength(50)]
        public string? KimlikNo { get; set; }

        public int? DogumGunu { get; set; }

        public int? DogumAyi { get; set; }

        [StringLength(10)]
        public string? DogumYili { get; set; }

        [StringLength(100)]
        public string? Ad { get; set; }

        [StringLength(100)]
        public string? Soyad { get; set; }

        [StringLength(50)]
        public string? EvTelefonu { get; set; }

        [StringLength(50)]
        public string? CepTelefonu { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? IletisimAdresi { get; set; }

        [StringLength(2000)]
        public string? BasvuruMetni { get; set; }

        [StringLength(50)]
        public string? CevapNasilVerilsin { get; set; }

        // Tüzel için alanlar
        [StringLength(500)]
        public string? Unvan { get; set; }

        [StringLength(200)]
        public string? VergiDairesi { get; set; }

        [StringLength(50)]
        public string? VergiNo { get; set; }

        [StringLength(50)]
        public string? IsTelefonu { get; set; }

        [StringLength(50)]
        public string? TuzelCepTelefonu { get; set; }

        [StringLength(200)]
        public string? TuzelEmail { get; set; }

        [StringLength(500)]
        public string? TuzelIletisimAdresi { get; set; }

        [StringLength(2000)]
        public string? TuzelBasvuruMetni { get; set; }

        [StringLength(50)]
        public string? TuzelCevapNasilVerilsin { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;
    }
}

