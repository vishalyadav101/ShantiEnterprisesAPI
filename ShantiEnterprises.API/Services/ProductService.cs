using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }


        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToResponse).ToList();
        }


        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return MapToResponse(product);
        }


        public async Task<ProductResponseDto> CreateAsync(
            ProductCreateDto dto)
        {
            // Check category
            var category =
                await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // Check duplicate SKU
            var existingProduct =
                await _productRepository.GetBySkuAsync(dto.SKU);

            if (existingProduct != null)
            {
                throw new Exception(
                    "Product with this SKU already exists.");
            }

            // Validate wholesale price
            if (dto.WholesalePrice > dto.MRP)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than MRP.");
            }

            var product = new Product
            {
                ProductName = dto.ProductName.Trim(),

                Description =
                    dto.Description?.Trim() ?? string.Empty,

                CategoryId = dto.CategoryId,

                MRP = dto.MRP,

                WholesalePrice = dto.WholesalePrice,

                Stock = dto.Stock,

                GSTPercentage = dto.GSTPercentage,

                SKU = dto.SKU.Trim().ToUpper(),

                ImageUrl = dto.ImageUrl,

                IsActive = true,

                CreatedDate = DateTime.UtcNow
            };

            var createdProduct =
                await _productRepository.AddAsync(product);

            return MapToResponse(createdProduct);
        }


        public async Task<ProductResponseDto?> UpdateAsync(
            int id,
            ProductUpdateDto dto)
        {
            var existingProduct =
                await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return null;
            }

            // Check category
            var category =
                await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null)
            {
                throw new Exception("Category not found.");
            }

            // Check SKU
            var productWithSameSku =
                await _productRepository.GetBySkuAsync(dto.SKU);

            if (productWithSameSku != null &&
                productWithSameSku.ProductId != id)
            {
                throw new Exception(
                    "Another product with this SKU already exists.");
            }

            // Validate price
            if (dto.WholesalePrice > dto.MRP)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than MRP.");
            }

            existingProduct.ProductName =
                dto.ProductName.Trim();

            existingProduct.Description =
                dto.Description?.Trim() ?? string.Empty;

            existingProduct.CategoryId =
                dto.CategoryId;

            existingProduct.MRP =
                dto.MRP;

            existingProduct.WholesalePrice =
                dto.WholesalePrice;

            existingProduct.Stock =
                dto.Stock;

            existingProduct.GSTPercentage =
                dto.GSTPercentage;

            existingProduct.SKU =
                dto.SKU.Trim().ToUpper();

            existingProduct.ImageUrl =
                dto.ImageUrl;

            existingProduct.IsActive =
                dto.IsActive;

            var updatedProduct =
                await _productRepository.UpdateAsync(existingProduct);

            if (updatedProduct == null)
            {
                return null;
            }

            return MapToResponse(updatedProduct);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }


        public async Task<ProductDetailResponseDto?>
    GetDetailsByIdAsync(int id)
        {
            var product =
                await _productRepository.GetDetailsByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDetailResponseDto
            {
                ProductId = product.ProductId,

                ProductName = product.ProductName,

                Description = product.Description,

                CategoryId = product.CategoryId,

                CategoryName =
                    product.Category?.CategoryName ?? string.Empty,

                MRP = product.MRP,

                WholesalePrice = product.WholesalePrice,

                Stock = product.Stock,

                GSTPercentage = product.GSTPercentage,

                SKU = product.SKU,

                IsActive = product.IsActive,

                CreatedDate = product.CreatedDate,

                Images = product.ProductImages
                    .Select(x => new ProductImageResponseDto
                    {
                        ProductImageId = x.ProductImageId,
                        ProductId = x.ProductId,
                        ImageUrl = x.ImageUrl,
                        IsPrimary = x.IsPrimary
                    })
                    .ToList(),

                PriceTiers = product.PriceTiers
                    .OrderBy(x => x.MinQuantity)
                    .Select(x => new ProductPriceTierResponseDto
                    {
                        ProductPriceTierId =
                            x.ProductPriceTierId,

                        ProductId =
                            x.ProductId,

                        MinQuantity =
                            x.MinQuantity,

                        MaxQuantity =
                            x.MaxQuantity,

                        Price =
                            x.Price
                    })
                    .ToList()
            };
        }


        private static ProductResponseDto MapToResponse(
            Product product)
        {
            return new ProductResponseDto
            {
                ProductId = product.ProductId,

                ProductName = product.ProductName,

                Description = product.Description,

                CategoryId = product.CategoryId,

                CategoryName =
                    product.Category?.CategoryName ?? string.Empty,

                MRP = product.MRP,

                WholesalePrice = product.WholesalePrice,

                Stock = product.Stock,

                GSTPercentage = product.GSTPercentage,

                SKU = product.SKU,

                ImageUrl = product.ImageUrl,

                IsActive = product.IsActive,

                CreatedDate = product.CreatedDate
            };
        }
    }
}