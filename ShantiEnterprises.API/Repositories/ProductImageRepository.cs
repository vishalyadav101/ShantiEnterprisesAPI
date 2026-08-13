using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ProductImageRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(
            int productId)
        {
            return await _context.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.ProductImageId)
                .ToListAsync();
        }

        public async Task<ProductImage> AddAsync(
            ProductImage image)
        {
            _context.ProductImages.Add(image);

            await _context.SaveChangesAsync();

            return image;
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(x =>
                    x.ProductImageId == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(x =>
                    x.ProductImageId == id);

            if (image == null)
            {
                return false;
            }

            _context.ProductImages.Remove(image);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}