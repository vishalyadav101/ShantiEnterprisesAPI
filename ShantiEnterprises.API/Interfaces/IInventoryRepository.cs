using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(int productId);

        Task<List<InventoryTransaction>>
            GetTransactionsByProductIdAsync(
                int productId);

        InventoryTransaction AddTransaction(
            InventoryTransaction transaction);

        Task SaveChangesAsync();
    }
}