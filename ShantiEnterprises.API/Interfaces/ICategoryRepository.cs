using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task<Category?> GetByNameAsync(string name);

        Task<Category> AddAsync(Category category);

        Task<Category?> UpdateAsync(Category category);

        Task<bool> DeleteAsync(int id);
    }
}