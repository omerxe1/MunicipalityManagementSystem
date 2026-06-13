namespace kocaaliv2.Models
{
    /// <summary>
    /// Duyuru detay sayfasında hem Announcement hem Duyuru kayıtlarını göstermek için ortak view model.
    /// </summary>
    public class DuyuruDetayViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public string? ImageUrl { get; set; }
    }
}
