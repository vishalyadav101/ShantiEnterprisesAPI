using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class InventoryRepository
        : IInventoryRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public InventoryRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL PRODUCTS
        // =========================

        public async Task<List<Product>>
            GetAllProductsAsync()
        {
            return await _context.Products
                .Include(x => x.Category)
                .OrderBy(x => x.ProductName)
                .ToListAsync();
        }

        // =========================
        // GET PRODUCT BY ID
        // =========================

        public async Task<Product?>
            GetProductByIdAsync(
                int productId)
        {
            return await _context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(
                    x => x.ProductId == productId);
        }

        // =========================
        // GET TRANSACTIONS
        // =========================

        public async Task<List<InventoryTransaction>>
            GetTransactionsByProductIdAsync(
                int productId)
        {
            return await _context
                .InventoryTransactions
                .Where(x =>
                    x.ProductId == productId)
                .OrderByDescending(
                    x => x.CreatedDate)
                .ToListAsync();
        }

        // =========================
        // ADD TRANSACTION
        // =========================

        public InventoryTransaction AddTransaction(
            InventoryTransaction transaction)
        {
            _context.InventoryTransactions.Add(
                transaction);

            return transaction;
        }

        // =========================
        // SAVE CHANGES
        // =========================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}