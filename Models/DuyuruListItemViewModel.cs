namespace kocaaliv2.Models
{
    /// <summary>
    /// Duyurular listesi sayfasında hem Announcements hem Duyurular tablosundan gelen kayıtları göstermek için kullanılır.
    /// </summary>
    public class DuyuruListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        /// <summary> "Announcement" veya "Duyuru" - hangi tablodan geldiği</summary>
        public string Kaynak { get; set; } = "Announcement";
    }
}
