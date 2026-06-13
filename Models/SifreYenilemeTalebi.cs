using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kocaaliv2.Models
{
    /// <summary>
    /// Şifre yenileme talepleri modeli
    /// </summary>
    public class SifreYenilemeTalebi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        [Column("KullaniciAdi")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Column("TalepTarihi")]
        public DateTime TalepTarihi { get; set; } = DateTime.Now;

        [Column("Durum")]
        [StringLength(20)]
        public string Durum { get; set; } = "Beklemede"; // Beklemede, Onaylandı, Reddedildi

        [Column("IslemTarihi")]
        public DateTime? IslemTarihi { get; set; }

        [Column("IslemYapan")]
        [StringLength(50)]
        public string? IslemYapan { get; set; } // Baş admin kullanıcı adı

        [Column("IpAdresi")]
        [StringLength(50)]
        public string? IpAdresi { get; set; }
    }
}



