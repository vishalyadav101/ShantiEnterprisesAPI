using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IBannerRepository
    {
        Task<List<Banner>> GetAllAsync();

        Task<Banner?> GetByIdAsync(int id);

        Task<Banner> AddAsync(Banner banner);

        Task UpdateAsync(Banner banner);

        Task<bool> DeleteAsync(int id);
    }
}