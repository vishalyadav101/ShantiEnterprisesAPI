using ShantiEnterprises.API.DTOs.Product;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IProductPriceTierService
    {
        Task<List<ProductPriceTierResponseDto>>
            GetByProductIdAsync(int productId);

        Task<ProductPriceTierResponseDto>
            CreateAsync(ProductPriceTierCreateDto dto);

        Task<ProductPriceTierResponseDto?>
            UpdateAsync(
                int id,
                ProductPriceTierCreateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}