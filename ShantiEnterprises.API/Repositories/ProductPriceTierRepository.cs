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

        public async Task<List<ProductPriceTier>> GetByProductIdAsync(
            int productId)
        {
            return await _context.ProductPriceTiers
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.MinQuantity)
                .ToListAsync();
        }

        public async Task<ProductPriceTier?> GetByIdAsync(int id)
        {
            return await _context.ProductPriceTiers
                .FirstOrDefaultAsync(x =>
                    x.ProductPriceTierId == id);
        }

        public async Task<ProductPriceTier> AddAsync(
            ProductPriceTier tier)
        {
            _context.ProductPriceTiers.Add(tier);

            await _context.SaveChangesAsync();

            return tier;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tier = await _context.ProductPriceTiers
                .FirstOrDefaultAsync(x =>
                    x.ProductPriceTierId == id);

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