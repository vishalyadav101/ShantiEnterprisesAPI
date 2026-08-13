using ShantiEnterprises.API.DTOs.Category;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();

        Task<CategoryResponseDto?> GetByIdAsync(int id);

        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);

        Task<CategoryResponseDto?> UpdateAsync(
            int id,
            CategoryUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}