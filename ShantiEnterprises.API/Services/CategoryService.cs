using ShantiEnterprises.API.DTOs.Category;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public CategoryService(
            ICategoryRepository repository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }


        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories
                .Select(MapToDto)
                .ToList();
        }


        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return MapToDto(category);
        }


        // =========================================================
        // CREATE
        // =========================================================

        public async Task<CategoryResponseDto> CreateAsync(
            CategoryCreateDto dto)
        {
            var categoryName =
                dto.CategoryName.Trim();


            // =====================================================
            // DUPLICATE CATEGORY CHECK
            // =====================================================

            var existingCategory =
                await _repository.GetByNameAsync(categoryName);

            if (existingCategory != null)
            {
                throw new Exception(
                    "Category with this name already exists.");
            }


            // =====================================================
            // IMAGE UPLOAD
            // =====================================================

            string? imageUrl = null;

            if (dto.ImageFile != null)
            {
                imageUrl =
                    await SaveImageAsync(dto.ImageFile);
            }


            // =====================================================
            // CREATE CATEGORY
            // =====================================================

            var category = new Category
            {
                CategoryName = categoryName,

                Description =
                    dto.Description?.Trim()
                    ?? string.Empty,

                ImageUrl = imageUrl,

                IsActive = true,

                CreatedDate = DateTime.UtcNow
            };


            var createdCategory =
                await _repository.AddAsync(category);


            return MapToDto(createdCategory);
        }


        // =========================================================
        // UPDATE
        // =========================================================

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


            var categoryName =
                dto.CategoryName.Trim();


            // =====================================================
            // DUPLICATE CATEGORY CHECK
            // =====================================================

            var duplicateCategory =
                await _repository.GetByNameAsync(
                    categoryName);

            if (
                duplicateCategory != null &&
                duplicateCategory.CategoryId != id
            )
            {
                throw new Exception(
                    "Another category with this name already exists.");
            }


            // =====================================================
            // UPDATE BASIC DETAILS
            // =====================================================

            existingCategory.CategoryName =
                categoryName;

            existingCategory.Description =
                dto.Description?.Trim()
                ?? string.Empty;

            existingCategory.IsActive =
                dto.IsActive;


            // =====================================================
            // IMAGE UPDATE
            // =====================================================

            if (dto.ImageFile != null)
            {
                var oldImageUrl =
                    existingCategory.ImageUrl;


                var newImageUrl =
                    await SaveImageAsync(
                        dto.ImageFile);


                existingCategory.ImageUrl =
                    newImageUrl;


                // Delete old uploaded category image
                DeleteImage(oldImageUrl);
            }


            // =====================================================
            // SAVE UPDATE
            // =====================================================

            var updatedCategory =
                await _repository.UpdateAsync(
                    existingCategory);

            if (updatedCategory == null)
            {
                return null;
            }


            return MapToDto(updatedCategory);
        }


        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var category =
                await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }


            var imageUrl =
                category.ImageUrl;


            var deleted =
                await _repository.DeleteAsync(id);


            if (deleted)
            {
                DeleteImage(imageUrl);
            }


            return deleted;
        }


        // =========================================================
        // SAVE IMAGE
        // =========================================================

        private async Task<string> SaveImageAsync(
            IFormFile imageFile)
        {
            // -----------------------------------------------------
            // MAXIMUM FILE SIZE = 5 MB
            // -----------------------------------------------------

            const long maxFileSize =
                5 * 1024 * 1024;


            if (imageFile.Length > maxFileSize)
            {
                throw new Exception(
                    "Category image size must be less than 5 MB.");
            }


            // -----------------------------------------------------
            // FILE EXTENSION
            // -----------------------------------------------------

            var extension =
                Path.GetExtension(
                    imageFile.FileName)
                .ToLowerInvariant();


            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


            if (!allowedExtensions.Contains(
                    extension))
            {
                throw new Exception(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }


            // -----------------------------------------------------
            // UPLOAD FOLDER
            // -----------------------------------------------------

            var uploadsFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "categories");


            if (!Directory.Exists(
                    uploadsFolder))
            {
                Directory.CreateDirectory(
                    uploadsFolder);
            }


            // -----------------------------------------------------
            // UNIQUE FILE NAME
            // -----------------------------------------------------

            var fileName =
                $"{Guid.NewGuid()}{extension}";


            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName);


            // -----------------------------------------------------
            // SAVE IMAGE
            // -----------------------------------------------------

            await using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create);


            await imageFile.CopyToAsync(
                stream);


            // -----------------------------------------------------
            // DATABASE URL
            // -----------------------------------------------------

            return
                $"/uploads/categories/{fileName}";
        }


        // =========================================================
        // DELETE IMAGE FILE
        // =========================================================

        private void DeleteImage(
            string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(
                    imageUrl))
            {
                return;
            }


            // -----------------------------------------------------
            // Only delete our category uploads
            // -----------------------------------------------------

            if (!imageUrl.StartsWith(
                    "/uploads/categories/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }


            var fileName =
                Path.GetFileName(
                    imageUrl);


            if (string.IsNullOrWhiteSpace(
                    fileName))
            {
                return;
            }


            var filePath =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "categories",
                    fileName);


            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        // =========================================================
        // ENTITY → DTO
        // =========================================================

        private static CategoryResponseDto MapToDto(
            Category category)
        {
            return new CategoryResponseDto
            {
                CategoryId =
                    category.CategoryId,

                CategoryName =
                    category.CategoryName,

                Description =
                    category.Description,

                ImageUrl =
                    category.ImageUrl,

                IsActive =
                    category.IsActive,

                CreatedDate =
                    category.CreatedDate
            };
        }
    }
}