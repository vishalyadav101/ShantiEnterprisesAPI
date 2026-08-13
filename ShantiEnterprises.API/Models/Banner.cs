namespace ShantiEnterprises.API.Models
{
    public class Banner
    {
        public int BannerId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string? ButtonText { get; set; }

        public string? ButtonUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}