using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class IstekVeOneri
    {
        public int Id { get; set; }

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
        public string? CepTelefonu { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Başvuru Metni zorunludur.")]
        [StringLength(2000, ErrorMessage = "Başvuru Metni en fazla 2000 karakter olabilir.")]
        public string BasvuruMetni { get; set; } = string.Empty;

        public bool KisiBilgilerimiGizle { get; set; } = false;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;
    }
}

