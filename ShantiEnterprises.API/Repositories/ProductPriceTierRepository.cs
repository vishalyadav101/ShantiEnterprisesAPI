using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ProductPriceTierRepository
        : IProductPriceTierRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ProductPriceTierRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY PRODUCT
        // =========================================================

        public async Task<List<ProductPriceTier>>
            GetByProductIdAsync(int productId)
        {
            return await _context.ProductPriceTiers
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.MinQuantity)
                .ToListAsync();
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<ProductPriceTier?>
            GetByIdAsync(int id)
        {
            return await _context.ProductPriceTiers
                .FirstOrDefaultAsync(
                    x => x.ProductPriceTierId == id);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<ProductPriceTier>
            AddAsync(ProductPriceTier tier)
        {
            _context.ProductPriceTiers.Add(tier);

            await _context.SaveChangesAsync();

            return tier;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<ProductPriceTier?>
            UpdateAsync(ProductPriceTier tier)
        {
            var existingTier =
                await _context.ProductPriceTiers
                    .FirstOrDefaultAsync(
                        x => x.ProductPriceTierId ==
                             tier.ProductPriceTierId);

            if (existingTier == null)
            {
                return null;
            }

            existingTier.MinQuantity =
                tier.MinQuantity;

            existingTier.MaxQuantity =
                tier.MaxQuantity;

            existingTier.Price =
                tier.Price;

            await _context.SaveChangesAsync();

            return existingTier;
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool>
            DeleteAsync(int id)
        {
            var tier =
                await _context.ProductPriceTiers
                    .FirstOrDefaultAsync(
                        x => x.ProductPriceTierId == id);

            if (tier == null)
            {
                return false;
            }

            _context.ProductPriceTiers.Remove(tier);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}