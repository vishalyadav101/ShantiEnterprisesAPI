using Microsoft.AspNetCore.Http;

namespace ShantiEnterprises.API.DTOs.Banner
{
    public class BannerUpdateDto
    {
        public string Title { get; set; }
            = string.Empty;

        public string Subtitle { get; set; }
            = string.Empty;

        // Optional during update.
        // If no new image is selected,
        // existing image will remain unchanged.
        public IFormFile? Image { get; set; }

        public string? ButtonText { get; set; }

        public string? ButtonUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}