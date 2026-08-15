namespace ShantiEnterprises.API.DTOs.Banner
{
    public class BannerUpdateDto
    {
        public string Title { get; set; }
            = string.Empty;

        public string Subtitle { get; set; }
            = string.Empty;

        public string ImageUrl { get; set; }
            = string.Empty;

        public string? ButtonText { get; set; }

        public string? ButtonUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}