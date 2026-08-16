using ShantiEnterprises.API.DTOs.Inventory;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryResponseDto>>
            GetAllAsync();

        Task<InventoryResponseDto?>
            GetByProductIdAsync(int productId);

        Task<List<InventoryTransactionResponseDto>>
            GetTransactionsAsync(int productId);

        Task<InventoryResponseDto>
            StockInAsync(StockInDto dto);

        Task<InventoryResponseDto>
            AdjustStockAsync(StockAdjustmentDto dto);
    }
}