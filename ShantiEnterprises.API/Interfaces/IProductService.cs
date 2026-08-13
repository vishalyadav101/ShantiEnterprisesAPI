using ShantiEnterprises.API.DTOs.Product;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponseDto>> GetAllAsync();

        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductDetailResponseDto?> GetDetailsByIdAsync(int id);

        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);

        Task<ProductResponseDto?> UpdateAsync(
            int id,
            ProductUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}