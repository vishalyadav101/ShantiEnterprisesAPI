using ShantiEnterprises.API.DTOs.Category;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }


        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories.Select(x => new CategoryResponseDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                IsActive = x.IsActive,
                CreatedDate = x.CreatedDate
            }).ToList();
        }


        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive,
                CreatedDate = category.CreatedDate
            };
        }


        public async Task<CategoryResponseDto> CreateAsync(
            CategoryCreateDto dto)
        {
            var existingCategory =
                await _repository.GetByNameAsync(dto.CategoryName);

            if (existingCategory != null)
            {
                throw new Exception(
                    "Category with this name already exists.");
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var createdCategory =
                await _repository.AddAsync(category);

            return new CategoryResponseDto
            {
                CategoryId = createdCategory.CategoryId,
                CategoryName = createdCategory.CategoryName,
                Description = createdCategory.Description,
                ImageUrl = createdCategory.ImageUrl,
                IsActive = createdCategory.IsActive,
                CreatedDate = createdCategory.CreatedDate
            };
        }


        public async Task<CategoryResponseDto?> UpdateAsync(
            int id,
            CategoryUpdateDto dto)
        {
            var existingCategory =
                await _repository.GetByIdAsync(id);

            if (existingCategory == null)
            {
                return null;
            }

            var duplicateCategory =
                await _repository.GetByNameAsync(dto.CategoryName);

            if (duplicateCategory != null &&
                duplicateCategory.CategoryId != id)
            {
                throw new Exception(
                    "Another category with this name already exists.");
            }

            existingCategory.CategoryName =
                dto.CategoryName.Trim();

            existingCategory.Description =
                dto.Description?.Trim() ?? string.Empty;

            existingCategory.ImageUrl =
                dto.ImageUrl;

            existingCategory.IsActive =
                dto.IsActive;

            var updatedCategory =
                await _repository.UpdateAsync(existingCategory);

            if (updatedCategory == null)
            {
                return null;
            }

            return new CategoryResponseDto
            {
                CategoryId = updatedCategory.CategoryId,
                CategoryName = updatedCategory.CategoryName,
                Description = updatedCategory.Description,
                ImageUrl = updatedCategory.ImageUrl,
                IsActive = updatedCategory.IsActive,
                CreatedDate = updatedCategory.CreatedDate
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}