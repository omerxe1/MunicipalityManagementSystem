using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kocaaliv2.Models
{
    public class TakvimGunu
    {
        [Key]
        [Column("TakvimGunuID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tarih alanı zorunludur.")]
        [Column("Tarih")]
        public DateTime Tarih { get; set; }

        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Column("Baslik")]
        public string? Baslik { get; set; }
    }
}

