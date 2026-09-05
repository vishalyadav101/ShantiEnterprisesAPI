using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ProductRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL PRODUCTS
        // =========================================================

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        // =========================================================
        // GET PRODUCT BY ID
        // =========================================================

        public async Task<Product?> GetByIdAsync(
            int id)
        {
            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(
                    x => x.ProductId == id);
        }

        // =========================================================
        // GET PRODUCT DETAILS
        // =========================================================

        public async Task<Product?> GetDetailsByIdAsync(
            int id)
        {
            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.PriceTiers)
                .FirstOrDefaultAsync(
                    x => x.ProductId == id);
        }

        // =========================================================
        // GET PRODUCT BY SKU
        // =========================================================

        public async Task<Product?> GetBySkuAsync(
            string sku)
        {
            var normalizedSku =
                sku.Trim().ToUpper();

            return await _context.Products
                .FirstOrDefaultAsync(
                    x => x.SKU == normalizedSku);
        }

        // =========================================================
        // CREATE PRODUCT
        // =========================================================

        public async Task<Product> AddAsync(
            Product product)
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.PriceTiers)
                .FirstAsync(
                    x => x.ProductId == product.ProductId);
        }

        // =========================================================
        // UPDATE PRODUCT
        // =========================================================

        public async Task<Product?> UpdateAsync(
            Product product)
        {
            var existingProduct =
                await _context.Products
                    .FirstOrDefaultAsync(
                        x => x.ProductId ==
                             product.ProductId);

            if (existingProduct == null)
            {
                return null;
            }

            // -----------------------------------------------------
            // BASIC DETAILS
            // -----------------------------------------------------

            existingProduct.ProductName =
                product.ProductName;

            existingProduct.Description =
                product.Description;

            existingProduct.CategoryId =
                product.CategoryId;

            // -----------------------------------------------------
            // PRICING
            // -----------------------------------------------------

            existingProduct.MRP =
                product.MRP;

            existingProduct.RetailPrice =
                product.RetailPrice;

            existingProduct.WholesalePrice =
                product.WholesalePrice;

            // -----------------------------------------------------
            // SHIPPING
            // -----------------------------------------------------

            existingProduct.ShippingCharge =
                product.ShippingCharge;

            // -----------------------------------------------------
            // STOCK
            // -----------------------------------------------------

            existingProduct.Stock =
                product.Stock;

            existingProduct.ReorderLevel =
                product.ReorderLevel;

            // -----------------------------------------------------
            // GST
            // -----------------------------------------------------

            existingProduct.GSTPercentage =
                product.GSTPercentage;

            // -----------------------------------------------------
            // OTHER
            // -----------------------------------------------------

            existingProduct.SKU =
                product.SKU;

            existingProduct.ImageUrl =
                product.ImageUrl;

            existingProduct.IsActive =
                product.IsActive;

            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.PriceTiers)
                .FirstOrDefaultAsync(
                    x => x.ProductId ==
                         product.ProductId);
        }

        // =========================================================
        // DELETE PRODUCT
        // =========================================================

        public async Task<bool> DeleteAsync(
            int id)
        {
            var product =
                await _context.Products
                    .FirstOrDefaultAsync(
                        x => x.ProductId == id);

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