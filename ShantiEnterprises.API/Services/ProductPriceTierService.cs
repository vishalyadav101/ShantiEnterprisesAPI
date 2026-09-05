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

        // =========================================================
        // GET TIERS BY PRODUCT
        // =========================================================

        public async Task<List<ProductPriceTierResponseDto>>
            GetByProductIdAsync(int productId)
        {
            var product =
                await _productRepository.GetByIdAsync(
                    productId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            var tiers =
                await _repository.GetByProductIdAsync(
                    productId);

            return tiers
                .OrderBy(x => x.MinQuantity)
                .Select(Map)
                .ToList();
        }

        // =========================================================
        // CREATE PRICE TIER
        // =========================================================

        public async Task<ProductPriceTierResponseDto>
            CreateAsync(
                ProductPriceTierCreateDto dto)
        {
            var product =
                await _productRepository.GetByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            ValidateBasicValues(dto);

            ValidatePrice(
                dto.Price,
                product);

            var existingTiers =
                await _repository.GetByProductIdAsync(
                    dto.ProductId);

            ValidateOverlap(
                dto,
                existingTiers);

            var tier =
                new ProductPriceTier
                {
                    ProductId =
                        dto.ProductId,

                    MinQuantity =
                        dto.MinQuantity,

                    MaxQuantity =
                        dto.MaxQuantity,

                    Price =
                        dto.Price
                };

            var result =
                await _repository.AddAsync(tier);

            return Map(result);
        }

        // =========================================================
        // UPDATE PRICE TIER
        // =========================================================

        public async Task<ProductPriceTierResponseDto?>
            UpdateAsync(
                int id,
                ProductPriceTierCreateDto dto)
        {
            // -----------------------------------------------------
            // GET EXISTING TIER
            // -----------------------------------------------------

            var existingTier =
                await _repository.GetByIdAsync(id);

            if (existingTier == null)
            {
                return null;
            }

            // -----------------------------------------------------
            // PRODUCT ID SECURITY / VALIDATION
            // -----------------------------------------------------

            if (existingTier.ProductId != dto.ProductId)
            {
                throw new Exception(
                    "Price tier does not belong to the specified product.");
            }

            var product =
                await _productRepository.GetByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            // -----------------------------------------------------
            // BASIC VALIDATION
            // -----------------------------------------------------

            ValidateBasicValues(dto);

            // -----------------------------------------------------
            // PRICE VALIDATION
            // -----------------------------------------------------

            ValidatePrice(
                dto.Price,
                product);

            // -----------------------------------------------------
            // GET EXISTING TIERS
            // -----------------------------------------------------

            var existingTiers =
                await _repository.GetByProductIdAsync(
                    dto.ProductId);

            // -----------------------------------------------------
            // REMOVE CURRENT TIER FROM OVERLAP CHECK
            // -----------------------------------------------------

            var otherTiers =
                existingTiers
                    .Where(x =>
                        x.ProductPriceTierId != id)
                    .ToList();

            // -----------------------------------------------------
            // OVERLAP VALIDATION
            // -----------------------------------------------------

            ValidateOverlap(
                dto,
                otherTiers);

            // -----------------------------------------------------
            // UPDATE
            // -----------------------------------------------------

            existingTier.MinQuantity =
                dto.MinQuantity;

            existingTier.MaxQuantity =
                dto.MaxQuantity;

            existingTier.Price =
                dto.Price;

            var updatedTier =
                await _repository.UpdateAsync(
                    existingTier);

            if (updatedTier == null)
            {
                return null;
            }

            return Map(updatedTier);
        }

        // =========================================================
        // DELETE PRICE TIER
        // =========================================================

        public async Task<bool>
            DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // =========================================================
        // BASIC VALIDATION
        // =========================================================

        private static void ValidateBasicValues(
            ProductPriceTierCreateDto dto)
        {
            if (dto.MinQuantity < 1)
            {
                throw new Exception(
                    "Minimum quantity must be at least 1.");
            }

            if (dto.MaxQuantity.HasValue &&
                dto.MaxQuantity.Value < dto.MinQuantity)
            {
                throw new Exception(
                    "Maximum quantity cannot be less than minimum quantity.");
            }

            if (dto.Price <= 0)
            {
                throw new Exception(
                    "Price must be greater than zero.");
            }
        }

        // =========================================================
        // PRICE VALIDATION
        // =========================================================

        private static void ValidatePrice(
            decimal price,
            Product product)
        {
            if (price > product.MRP)
            {
                throw new Exception(
                    "Tier price cannot be greater than MRP.");
            }

            if (price > product.RetailPrice)
            {
                throw new Exception(
                    "Tier price cannot be greater than retail price.");
            }
        }

        // =========================================================
        // OVERLAP VALIDATION
        // =========================================================

        private static void ValidateOverlap(
            ProductPriceTierCreateDto dto,
            List<ProductPriceTier> existingTiers)
        {
            foreach (var existingTier in existingTiers)
            {
                if (RangesOverlap(
                    dto.MinQuantity,
                    dto.MaxQuantity,
                    existingTier.MinQuantity,
                    existingTier.MaxQuantity))
                {
                    throw new Exception(
                        $"Quantity range " +
                        $"{dto.MinQuantity}-{FormatMax(dto.MaxQuantity)} " +
                        $"overlaps with existing tier " +
                        $"{existingTier.MinQuantity}-{FormatMax(existingTier.MaxQuantity)}.");
                }
            }
        }

        // =========================================================
        // RANGE OVERLAP CHECK
        // =========================================================

        private static bool RangesOverlap(
            int newMin,
            int? newMax,
            int existingMin,
            int? existingMax)
        {
            int newEnd =
                newMax ?? int.MaxValue;

            int existingEnd =
                existingMax ?? int.MaxValue;

            return newMin <= existingEnd &&
                   existingMin <= newEnd;
        }

        // =========================================================
        // MAP ENTITY -> DTO
        // =========================================================

        private static ProductPriceTierResponseDto Map(
            ProductPriceTier tier)
        {
            return new ProductPriceTierResponseDto
            {
                ProductPriceTierId =
                    tier.ProductPriceTierId,

                ProductId =
                    tier.ProductId,

                MinQuantity =
                    tier.MinQuantity,

                MaxQuantity =
                    tier.MaxQuantity,

                Price =
                    tier.Price
            };
        }

        // =========================================================
        // FORMAT MAX
        // =========================================================

        private static string FormatMax(
            int? maxQuantity)
        {
            return maxQuantity?.ToString() ?? "+";
        }
    }
}