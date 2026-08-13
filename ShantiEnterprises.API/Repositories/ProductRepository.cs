using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ProductRepository(ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductId == id);
        }


        public async Task<Product?> GetDetailsByIdAsync(int id)
        {
            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.PriceTiers)
                .FirstOrDefaultAsync(x => x.ProductId == id);
        }


        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .FirstOrDefaultAsync(x => x.SKU == sku);
        }


        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(x => x.Category)
                .FirstAsync(x => x.ProductId == product.ProductId);
        }


        public async Task<Product?> UpdateAsync(Product product)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.ProductId == product.ProductId);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.MRP = product.MRP;
            existingProduct.WholesalePrice = product.WholesalePrice;
            existingProduct.Stock = product.Stock;
            existingProduct.GSTPercentage = product.GSTPercentage;
            existingProduct.SKU = product.SKU;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.IsActive = product.IsActive;

            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x =>
                    x.ProductId == product.ProductId);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}