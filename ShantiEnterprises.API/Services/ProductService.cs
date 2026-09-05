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

        // =========================================================
        // GET ALL PRODUCTS
        // =========================================================

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var products =
                await _productRepository.GetAllAsync();

            return products
                .Select(MapToResponse)
                .ToList();
        }

        // =========================================================
        // GET PRODUCT BY ID
        // =========================================================

        public async Task<ProductResponseDto?> GetByIdAsync(
            int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return MapToResponse(product);
        }

        // =========================================================
        // CREATE PRODUCT
        // =========================================================

        public async Task<ProductResponseDto> CreateAsync(
            ProductCreateDto dto)
        {
            // -----------------------------------------------------
            // CATEGORY VALIDATION
            // -----------------------------------------------------

            var category =
                await _categoryRepository.GetByIdAsync(
                    dto.CategoryId);

            if (category == null)
            {
                throw new Exception(
                    "Category not found.");
            }

            // -----------------------------------------------------
            // SKU VALIDATION
            // -----------------------------------------------------

            var normalizedSku =
                dto.SKU.Trim().ToUpper();

            var existingProduct =
                await _productRepository.GetBySkuAsync(
                    normalizedSku);

            if (existingProduct != null)
            {
                throw new Exception(
                    "Product with this SKU already exists.");
            }

            // -----------------------------------------------------
            // PRICE VALIDATION
            // -----------------------------------------------------

            if (dto.RetailPrice > dto.MRP)
            {
                throw new Exception(
                    "Retail price cannot be greater than MRP.");
            }

            if (dto.WholesalePrice > dto.MRP)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than MRP.");
            }

            if (dto.WholesalePrice > dto.RetailPrice)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than retail price.");
            }

            // -----------------------------------------------------
            // SHIPPING VALIDATION
            // -----------------------------------------------------

            if (dto.ShippingCharge < 0)
            {
                throw new Exception(
                    "Shipping charge cannot be negative.");
            }

            // -----------------------------------------------------
            // CREATE PRODUCT
            // -----------------------------------------------------

            var product = new Product
            {
                ProductName =
                    dto.ProductName.Trim(),

                Description =
                    dto.Description?.Trim()
                    ?? string.Empty,

                CategoryId =
                    dto.CategoryId,

                MRP =
                    dto.MRP,

                RetailPrice =
                    dto.RetailPrice,

                WholesalePrice =
                    dto.WholesalePrice,

                ShippingCharge =
                    dto.ShippingCharge,

                Stock =
                    dto.Stock,

                GSTPercentage =
                    dto.GSTPercentage,

                SKU =
                    normalizedSku,

                ImageUrl =
                    dto.ImageUrl,

                IsActive =
                    true,

                CreatedDate =
                    DateTime.UtcNow
            };

            var createdProduct =
                await _productRepository.AddAsync(
                    product);

            return MapToResponse(
                createdProduct);
        }

        // =========================================================
        // UPDATE PRODUCT
        // =========================================================

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

            // -----------------------------------------------------
            // CATEGORY VALIDATION
            // -----------------------------------------------------

            var category =
                await _categoryRepository.GetByIdAsync(
                    dto.CategoryId);

            if (category == null)
            {
                throw new Exception(
                    "Category not found.");
            }

            // -----------------------------------------------------
            // SKU VALIDATION
            // -----------------------------------------------------

            var normalizedSku =
                dto.SKU.Trim().ToUpper();

            var productWithSameSku =
                await _productRepository.GetBySkuAsync(
                    normalizedSku);

            if (productWithSameSku != null &&
                productWithSameSku.ProductId != id)
            {
                throw new Exception(
                    "Another product with this SKU already exists.");
            }

            // -----------------------------------------------------
            // PRICE VALIDATION
            // -----------------------------------------------------

            if (dto.RetailPrice > dto.MRP)
            {
                throw new Exception(
                    "Retail price cannot be greater than MRP.");
            }

            if (dto.WholesalePrice > dto.MRP)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than MRP.");
            }

            if (dto.WholesalePrice > dto.RetailPrice)
            {
                throw new Exception(
                    "Wholesale price cannot be greater than retail price.");
            }

            // -----------------------------------------------------
            // SHIPPING VALIDATION
            // -----------------------------------------------------

            if (dto.ShippingCharge < 0)
            {
                throw new Exception(
                    "Shipping charge cannot be negative.");
            }

            // -----------------------------------------------------
            // UPDATE PRODUCT
            // -----------------------------------------------------

            existingProduct.ProductName =
                dto.ProductName.Trim();

            existingProduct.Description =
                dto.Description?.Trim()
                ?? string.Empty;

            existingProduct.CategoryId =
                dto.CategoryId;

            existingProduct.MRP =
                dto.MRP;

            existingProduct.RetailPrice =
                dto.RetailPrice;

            existingProduct.WholesalePrice =
                dto.WholesalePrice;

            existingProduct.ShippingCharge =
                dto.ShippingCharge;

            existingProduct.Stock =
                dto.Stock;

            existingProduct.GSTPercentage =
                dto.GSTPercentage;

            existingProduct.SKU =
                normalizedSku;

            existingProduct.ImageUrl =
                dto.ImageUrl;

            existingProduct.IsActive =
                dto.IsActive;

            var updatedProduct =
                await _productRepository.UpdateAsync(
                    existingProduct);

            if (updatedProduct == null)
            {
                return null;
            }

            return MapToResponse(
                updatedProduct);
        }

        // =========================================================
        // DELETE PRODUCT
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository
                .DeleteAsync(id);
        }

        // =========================================================
        // PRODUCT DETAILS
        // =========================================================

        public async Task<ProductDetailResponseDto?>
            GetDetailsByIdAsync(int id)
        {
            var product =
                await _productRepository
                    .GetDetailsByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDetailResponseDto
            {
                ProductId =
                    product.ProductId,

                ProductName =
                    product.ProductName,

                Description =
                    product.Description,

                CategoryId =
                    product.CategoryId,

                CategoryName =
                    product.Category?.CategoryName
                    ?? string.Empty,

                MRP =
                    product.MRP,

                RetailPrice =
                    product.RetailPrice,

                WholesalePrice =
                    product.WholesalePrice,

                ShippingCharge =
                    product.ShippingCharge,

                Stock =
                    product.Stock,

                GSTPercentage =
                    product.GSTPercentage,

                SKU =
                    product.SKU,

                IsActive =
                    product.IsActive,

                CreatedDate =
                    product.CreatedDate,

                Images =
                    product.ProductImages
                        .Select(x =>
                            new ProductImageResponseDto
                            {
                                ProductImageId =
                                    x.ProductImageId,

                                ProductId =
                                    x.ProductId,

                                ImageUrl =
                                    x.ImageUrl,

                                IsPrimary =
                                    x.IsPrimary
                            })
                        .ToList(),

                PriceTiers =
                    product.PriceTiers
                        .OrderBy(x => x.MinQuantity)
                        .Select(x =>
                            new ProductPriceTierResponseDto
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

        // =========================================================
        // RESPONSE MAPPING
        // =========================================================

        private static ProductResponseDto MapToResponse(
            Product product)
        {
            var primaryImage =
                product.ProductImages?
                    .FirstOrDefault(
                        x => x.IsPrimary);

            var firstImage =
                product.ProductImages?
                    .FirstOrDefault();

            return new ProductResponseDto
            {
                ProductId =
                    product.ProductId,

                ProductName =
                    product.ProductName,

                Description =
                    product.Description,

                CategoryId =
                    product.CategoryId,

                CategoryName =
                    product.Category?.CategoryName
                    ?? string.Empty,

                MRP =
                    product.MRP,

                RetailPrice =
                    product.RetailPrice,

                WholesalePrice =
                    product.WholesalePrice,

                ShippingCharge =
                    product.ShippingCharge,

                Stock =
                    product.Stock,

                GSTPercentage =
                    product.GSTPercentage,

                SKU =
                    product.SKU,

                ImageUrl =
                    primaryImage?.ImageUrl
                    ?? firstImage?.ImageUrl
                    ?? product.ImageUrl,

                IsActive =
                    product.IsActive,

                CreatedDate =
                    product.CreatedDate
            };
        }
    }
}