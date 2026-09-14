using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public ProductImageService(
            IProductImageRepository repository,
            IProductRepository productRepository,
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _productRepository = productRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ProductImageResponseDto> UploadAsync(
            int productId,
            ProductImageUploadDto dto)
        {
            // -----------------------------------------
            // Check Product
            // -----------------------------------------

            var product =
                await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            // -----------------------------------------
            // Validate Image
            // -----------------------------------------

            if (dto.Image == null || dto.Image.Length == 0)
            {
                throw new Exception("Please select an image.");
            }

            if (dto.Image.Length > MaxFileSize)
            {
                throw new Exception(
                    "Image size cannot be greater than 5 MB.");
            }

            var extension =
                Path.GetExtension(dto.Image.FileName)
                    .ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
            {
                throw new Exception(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");
            }

            // -----------------------------------------
            // Upload Image To Cloudinary
            // -----------------------------------------

            var uploadResult =
                await _cloudinaryService.UploadImageAsync(
                    dto.Image,
                    "shanti-enterprises/products"
                );

            var imageUrl = uploadResult.Url;

            // -----------------------------------------
            // Primary Image Handling
            // -----------------------------------------

            if (dto.IsPrimary)
            {
                var existingImages =
                    await _repository.GetByProductIdAsync(productId);

                foreach (var image in existingImages)
                {
                    image.IsPrimary = false;
                }
            }

            // -----------------------------------------
            // Create ProductImage
            // -----------------------------------------

            var productImage = new ProductImage
            {
                ProductId = productId,

                ImageUrl = imageUrl,

                IsPrimary = dto.IsPrimary
            };

            // -----------------------------------------
            // Save ProductImage
            // -----------------------------------------

            var savedImage =
                await _repository.AddAsync(productImage);

            // -----------------------------------------
            // Update Product Main Image
            // -----------------------------------------

            if (dto.IsPrimary)
            {
                product.ImageUrl = imageUrl;

                await _productRepository.UpdateAsync(product);
            }

            // -----------------------------------------
            // Return Response
            // -----------------------------------------

            return MapToResponse(savedImage);
        }

        public async Task<List<ProductImageResponseDto>>
            GetByProductIdAsync(int productId)
        {
            var images =
                await _repository.GetByProductIdAsync(productId);

            return images
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // -----------------------------------------
            // Get Image
            // -----------------------------------------

            var image =
                await _repository.GetByIdAsync(id);

            if (image == null)
            {
                return false;
            }

            // -----------------------------------------
            // Delete Database Record
            // -----------------------------------------

            var result =
                await _repository.DeleteAsync(id);

            // -----------------------------------------
            // Delete Image From Cloudinary
            // -----------------------------------------

            if (result)
            {
                var publicId =
                    ExtractCloudinaryPublicId(image.ImageUrl);

                if (!string.IsNullOrWhiteSpace(publicId))
                {
                    try
                    {
                        await _cloudinaryService
                            .DeleteImageAsync(publicId);
                    }
                    catch
                    {
                        // Ignore Cloudinary deletion errors.
                    }
                }
            }

            return result;
        }

        // -----------------------------------------
        // Map Response
        // -----------------------------------------

        private static ProductImageResponseDto MapToResponse(
            ProductImage image)
        {
            return new ProductImageResponseDto
            {
                ProductImageId =
                    image.ProductImageId,

                ProductId =
                    image.ProductId,

                ImageUrl =
                    image.ImageUrl,

                IsPrimary =
                    image.IsPrimary
            };
        }

        // -----------------------------------------
        // Extract Cloudinary Public ID
        // -----------------------------------------

        private static string? ExtractCloudinaryPublicId(
            string? imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return null;
                }

                var uri = new Uri(imageUrl);

                var path = uri.AbsolutePath;

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
                        uploadIndex + "/upload/".Length
                    );

                var parts =
                    publicPath.Split(
                        '/',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                // Remove Cloudinary version:
                // v123456789/
                if (parts.Length > 1 &&
                    parts[0].StartsWith("v") &&
                    long.TryParse(
                        parts[0].Substring(1),
                        out _))
                {
                    publicPath =
                        string.Join(
                            "/",
                            parts.Skip(1)
                        );
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
                            extension.Length
                        );
                }

                return publicPath.Trim('/');
            }
            catch
            {
                return null;
            }
        }
    }
}