namespace ShantiEnterprises.API.DTOs.WebsiteSetting
{
    public class WebsiteSettingResponseDto
    {
        public int WebsiteSettingId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public string? FaviconUrl { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? WhatsAppNumber { get; set; }

        public string? CustomerCare { get; set; }

        public string? GSTIN { get; set; }

        public string? Proprietor { get; set; }

        public int? SinceYear { get; set; }

        public string? Address { get; set; }

        public string? FacebookUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? YouTubeUrl { get; set; }
        public string? HighlightText { get; set; }

        public string? FooterText { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}