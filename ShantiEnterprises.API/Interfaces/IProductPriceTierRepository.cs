using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IProductPriceTierRepository
    {
        Task<List<ProductPriceTier>> GetByProductIdAsync(int productId);

        Task<ProductPriceTier?> GetByIdAsync(int id);

        Task<ProductPriceTier> AddAsync(ProductPriceTier tier);

        Task<bool> DeleteAsync(int id);
    }
}