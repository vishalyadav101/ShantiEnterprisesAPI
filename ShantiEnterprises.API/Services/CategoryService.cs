using ShantiEnterprises.API.DTOs.Category;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public CategoryService(
            ICategoryRepository repository,
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
        }

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories =
                await _repository.GetAllAsync();

            return categories
                .Select(MapToDto)
                .ToList();
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category =
                await _repository.GetByIdAsync(id);

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
                ValidateImage(dto.ImageFile);

                var uploadResult =
                    await _cloudinaryService.UploadImageAsync(
                        dto.ImageFile,
                        "shanti-enterprises/categories");

                imageUrl = uploadResult.Url;
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
                await _repository.GetByNameAsync(categoryName);

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
                ValidateImage(dto.ImageFile);

                var oldImageUrl =
                    existingCategory.ImageUrl;

                var uploadResult =
                    await _cloudinaryService.UploadImageAsync(
                        dto.ImageFile,
                        "shanti-enterprises/categories");

                existingCategory.ImageUrl =
                    uploadResult.Url;

                // Delete old Cloudinary image
                var oldPublicId =
                    ExtractCloudinaryPublicId(oldImageUrl);

                if (!string.IsNullOrWhiteSpace(oldPublicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(oldPublicId);
                    }
                    catch
                    {
                        // Ignore Cloudinary cleanup errors.
                    }
                }
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
                var publicId =
                    ExtractCloudinaryPublicId(imageUrl);

                if (!string.IsNullOrWhiteSpace(publicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(publicId);
                    }
                    catch
                    {
                        // Ignore Cloudinary cleanup errors.
                    }
                }
            }

            return deleted;
        }

        // =========================================================
        // VALIDATE IMAGE
        // =========================================================

        private void ValidateImage(
            IFormFile imageFile)
        {
            // -----------------------------------------------------
            // MAXIMUM FILE SIZE = 5 MB
            // -----------------------------------------------------

            if (imageFile.Length > MaxFileSize)
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

            if (!_allowedExtensions.Contains(extension))
            {
                throw new Exception(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }
        }

        // =========================================================
        // EXTRACT CLOUDINARY PUBLIC ID
        // =========================================================

        private static string? ExtractCloudinaryPublicId(
            string? imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return null;
                }

                var uri =
                    new Uri(imageUrl);

                var path =
                    uri.AbsolutePath;

                var uploadIndex =
                    path.IndexOf(
                        "/upload/",
                        StringComparison.OrdinalIgnoreCase);

                if (uploadIndex < 0)
                {
                    return null;
                }

                var publicPath =
                    path.Substring(
                        uploadIndex + "/upload/".Length);

                var parts =
                    publicPath.Split(
                        '/',
                        StringSplitOptions.RemoveEmptyEntries);

                // Remove Cloudinary version:
                // v123456789/
                if (
                    parts.Length > 1 &&
                    parts[0].StartsWith("v") &&
                    long.TryParse(
                        parts[0].Substring(1),
                        out _)
                )
                {
                    publicPath =
                        string.Join(
                            "/",
                            parts.Skip(1));
                }

                // Remove file extension
                var extension =
                    Path.GetExtension(publicPath);

                if (!string.IsNullOrEmpty(extension))
                {
                    publicPath =
                        publicPath.Substring(
                            0,
                            publicPath.Length -
                            extension.Length);
                }

                return publicPath.Trim('/');
            }
            catch
            {
                return null;
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