using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IWebsiteSettingRepository
    {
        Task<WebsiteSetting?> GetAsync();

        Task<WebsiteSetting> CreateAsync(
            WebsiteSetting setting);

        Task UpdateAsync(
            WebsiteSetting setting);
    }
}