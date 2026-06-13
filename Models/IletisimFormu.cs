using System.ComponentModel.DataAnnotations;

namespace kocaaliv2.Models
{
    public class IletisimFormu
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Kimlik No en fazla 50 karakter olabilir.")]
        public string KimlikNo { get; set; } = string.Empty;

        public int DogumGunu { get; set; }

        public int DogumAyi { get; set; }

        [StringLength(50, ErrorMessage = "Doğum yılı en fazla 50 karakter olabilir.")]
        public string DogumYili { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        public string Soyad { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Cep Telefonu en fazla 50 karakter olabilir.")]
        public string CepTelefonu { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Email en fazla 200 karakter olabilir.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Başvuru Metni zorunludur.")]
        [StringLength(2000, ErrorMessage = "Başvuru Metni en fazla 2000 karakter olabilir.")]
        public string BasvuruMetni { get; set; } = string.Empty;

        public bool KisiBilgilerimiGizle { get; set; } = false;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;
    }
}
