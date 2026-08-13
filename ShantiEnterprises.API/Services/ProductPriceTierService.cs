using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ProductPriceTierService
        : IProductPriceTierService
    {
        private readonly IProductPriceTierRepository _repository;
        private readonly IProductRepository _productRepository;

        public ProductPriceTierService(
            IProductPriceTierRepository repository,
            IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<List<ProductPriceTierResponseDto>>
            GetByProductIdAsync(int productId)
        {
            var tiers =
                await _repository.GetByProductIdAsync(productId);

            return tiers.Select(Map).ToList();
        }

        public async Task<ProductPriceTierResponseDto>
            CreateAsync(ProductPriceTierCreateDto dto)
        {
            var product =
                await _productRepository.GetByIdAsync(dto.ProductId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            if (dto.MaxQuantity.HasValue &&
                dto.MaxQuantity.Value < dto.MinQuantity)
            {
                throw new Exception(
                    "Maximum quantity cannot be less than minimum quantity.");
            }

            if (dto.Price > product.MRP)
            {
                throw new Exception(
                    "Wholesale tier price cannot be greater than MRP.");
            }

            var tier = new ProductPriceTier
            {
                ProductId = dto.ProductId,
                MinQuantity = dto.MinQuantity,
                MaxQuantity = dto.MaxQuantity,
                Price = dto.Price
            };

            var result = await _repository.AddAsync(tier);

            return Map(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static ProductPriceTierResponseDto Map(
            ProductPriceTier tier)
        {
            return new ProductPriceTierResponseDto
            {
                ProductPriceTierId = tier.ProductPriceTierId,
                ProductId = tier.ProductId,
                MinQuantity = tier.MinQuantity,
                MaxQuantity = tier.MaxQuantity,
                Price = tier.Price
            };
        }
    }
}