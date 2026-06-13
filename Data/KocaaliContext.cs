using Microsoft.EntityFrameworkCore;
using kocaaliv2.Models; 

namespace kocaaliv2.Data
{
    public class KocaaliContext : DbContext
    {
        public KocaaliContext(DbContextOptions<KocaaliContext> options)
            : base(options)
        {
        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Haber> Haberler { get; set; }
        public DbSet<Calisma> Calismalar { get; set; }
        public DbSet<Etkinlik> Etkinlikler { get; set; }
        public DbSet<TakvimGunu> TakvimGunleri { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Yonetmelik> Yonetmelikler { get; set; }
        public DbSet<EskiBaskan> EskiBaskanlar { get; set; }
        public DbSet<MeclisUyesi> MeclisUyeleri { get; set; }
        public DbSet<MeclisKarari> MeclisKararlari { get; set; }
        public DbSet<KardesSehir> KardesSehirler { get; set; }
        public DbSet<AfadAcilToplanmaAlani> AfadAcilToplanmaAlanlari { get; set; }
        public DbSet<Ihale> Ihaleler { get; set; }
        public DbSet<FaaliyetRaporu> FaaliyetRaporlari { get; set; }
        public DbSet<ImarPlani> ImarPlanlari { get; set; }
        public DbSet<IcKontrolUyumEylemPlani> IcKontrolUyumEylemPlanlari { get; set; }
        public DbSet<Kvkk> KvkkDokumanlari { get; set; }
        public DbSet<PerformansProgrami> PerformansProgramlari { get; set; }
        public DbSet<StratejikPlan> StratejikPlanlar { get; set; }
        public DbSet<KaliteYonetimSistemi> KaliteYonetimSistemleri { get; set; }
        public DbSet<BasvuruKilavuzu> BasvuruKilavuzlari { get; set; }
        public DbSet<IletisimFormu> IletisimFormlari { get; set; }
        public DbSet<BilgiEdinmeBasvuru> BilgiEdinmeBasvurulari { get; set; }
        public DbSet<IstekVeOneri> IstekVeOneriler { get; set; }
        public DbSet<TelefonRehberi> TelefonRehberi { get; set; }
        public DbSet<YararliLink> YararliLinkler { get; set; }
        public DbSet<QuickMenuItem> QuickMenuItems { get; set; }
        public DbSet<PopupAnnouncement> PopupAnnouncements { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Duyuru> Duyurular { get; set; }
        public DbSet<IletisimBasvuru> IletisimBasvurulari { get; set; }
        
        // FAZ 4 - Güvenlik modelleri
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
           
            modelBuilder.Entity<Etkinlik>(entity =>
            {
                entity.ToTable("Etkinlikler");
                
                
                entity.Property(e => e.Id).HasColumnName("EtkinlikID");
                entity.Property(e => e.Baslik).HasColumnName("EtkinlikAdi");
                entity.Property(e => e.Aciklama).HasColumnName("EtkinlikAciklama");
                entity.Property(e => e.ResimYolu).HasColumnName("EtkinlikResimYolu");
                entity.Property(e => e.Tarih).HasColumnName("EtkinlikTarihi");
                entity.Property(e => e.Kategori).HasColumnName("EtkinlikKategori");
                entity.Property(e => e.Mekan).HasColumnName("EtkinlikMekan");
            });
            
           
            modelBuilder.Entity<TakvimGunu>(entity =>
            {
                entity.ToTable("TakvimGunleri");
                
                
                entity.Property(e => e.Id).HasColumnName("TakvimGunuID");
                entity.Property(e => e.Tarih).HasColumnName("Tarih");
                entity.Property(e => e.Baslik).HasColumnName("Baslik");
            });

            modelBuilder.Entity<Ihale>(entity =>
            {
                entity.ToTable("Ihaleler");
            });
        }
    }
}
