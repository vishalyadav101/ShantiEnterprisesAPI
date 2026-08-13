using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

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
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _productRepository = productRepository;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ProductImageResponseDto> UploadAsync(
            int productId,
            ProductImageUploadDto dto)
        {
            var product =
                await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }


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


            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "products"
            );


            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            var fileName =
                $"{Guid.NewGuid()}{extension}";


            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName
                );


            await using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }


            var request =
                _httpContextAccessor.HttpContext?.Request;


            var imageUrl =
                $"{request?.Scheme}://{request?.Host}/uploads/products/{fileName}";


            // If this image is primary,
            // make all existing images non-primary
            if (dto.IsPrimary)
            {
                var existingImages =
                    await _repository.GetByProductIdAsync(productId);

                foreach (var image in existingImages)
                {
                    image.IsPrimary = false;
                }
            }


            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                IsPrimary = dto.IsPrimary
            };


            var savedImage =
                await _repository.AddAsync(productImage);


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
            var image =
                await _repository.GetByIdAsync(id);

            if (image == null)
            {
                return false;
            }


            var result =
                await _repository.DeleteAsync(id);


            if (result)
            {
                DeletePhysicalFile(image.ImageUrl);
            }


            return result;
        }


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


        private void DeletePhysicalFile(string imageUrl)
        {
            try
            {
                var uri =
                    new Uri(imageUrl);

                var relativePath =
                    uri.AbsolutePath
                        .TrimStart('/')
                        .Replace(
                            '/',
                            Path.DirectorySeparatorChar
                        );

                var filePath =
                    Path.Combine(
                        _environment.WebRootPath,
                        relativePath
                    );

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Ignore physical file deletion errors.
            }
        }
    }
}