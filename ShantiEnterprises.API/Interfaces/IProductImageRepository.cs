using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);

        Task<ProductImage> AddAsync(ProductImage image);

        Task<ProductImage?> GetByIdAsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}