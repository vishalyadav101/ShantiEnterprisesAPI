using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class WebsiteSettingRepository
        : IWebsiteSettingRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public WebsiteSettingRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<WebsiteSetting?> GetAsync()
        {
            return await _context.WebsiteSettings
                .FirstOrDefaultAsync();
        }

        public async Task<WebsiteSetting> CreateAsync(
            WebsiteSetting setting)
        {
            _context.WebsiteSettings.Add(setting);

            await _context.SaveChangesAsync();

            return setting;
        }

        public async Task UpdateAsync(
            WebsiteSetting setting)
        {
            _context.WebsiteSettings.Update(setting);

            await _context.SaveChangesAsync();
        }
    }
}