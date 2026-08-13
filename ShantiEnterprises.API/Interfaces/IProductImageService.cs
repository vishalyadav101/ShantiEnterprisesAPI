using ShantiEnterprises.API.DTOs.Product;


namespace ShantiEnterprises.API.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageResponseDto> UploadAsync(
            int productId,
            ProductImageUploadDto dto);

        Task<List<ProductImageResponseDto>> GetByProductIdAsync(
            int productId);

        Task<bool> DeleteAsync(int id);
    }
}