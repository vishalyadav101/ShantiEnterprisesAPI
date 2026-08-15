using ShantiEnterprises.API.DTOs.WebsiteSetting;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IWebsiteSettingService
    {
        Task<WebsiteSettingResponseDto?> GetAsync();

        Task<WebsiteSettingResponseDto>
            SaveAsync(WebsiteSettingCreateUpdateDto dto);
    }
}