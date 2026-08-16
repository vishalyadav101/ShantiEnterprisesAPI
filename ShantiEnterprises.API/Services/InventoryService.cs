using ShantiEnterprises.API.DTOs.Inventory;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(
            IInventoryRepository repository)
        {
            _repository = repository;
        }

        // =========================
        // GET ALL INVENTORY
        // =========================

        public async Task<List<InventoryResponseDto>>
            GetAllAsync()
        {
            var products =
                await _repository.GetAllProductsAsync();

            return products
                .Select(MapInventory)
                .ToList();
        }

        // =========================
        // GET INVENTORY BY PRODUCT
        // =========================

        public async Task<InventoryResponseDto?>
            GetByProductIdAsync(int productId)
        {
            var product =
                await _repository.GetProductByIdAsync(
                    productId);

            if (product == null)
            {
                return null;
            }

            return MapInventory(product);
        }

        // =========================
        // GET TRANSACTION HISTORY
        // =========================

        public async Task<
            List<InventoryTransactionResponseDto>>
            GetTransactionsAsync(int productId)
        {
            var product =
                await _repository.GetProductByIdAsync(
                    productId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            var transactions =
                await _repository
                    .GetTransactionsByProductIdAsync(
                        productId);

            return transactions
                .Select(x => new InventoryTransactionResponseDto
                {
                    InventoryTransactionId =
                        x.InventoryTransactionId,

                    ProductId =
                        x.ProductId,

                    ProductName =
                        x.Product?.ProductName
                        ?? product.ProductName,

                    Quantity =
                        x.Quantity,

                    TransactionType =
                        x.TransactionType,

                    ReferenceId =
                        x.ReferenceId,

                    Remarks =
                        x.Remarks,

                    CreatedDate =
                        x.CreatedDate
                })
                .ToList();
        }

        // =========================
        // STOCK IN
        // =========================

        public async Task<InventoryResponseDto>
            StockInAsync(StockInDto dto)
        {
            if (dto.ProductId <= 0)
            {
                throw new Exception(
                    "Invalid product ID.");
            }

            if (dto.Quantity <= 0)
            {
                throw new Exception(
                    "Stock quantity must be greater than 0.");
            }

            var product =
                await _repository.GetProductByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            // Increase current stock
            product.Stock += dto.Quantity;

            // Create transaction history
            var transaction = new InventoryTransaction
            {
                ProductId =
                    product.ProductId,

                Quantity =
                    dto.Quantity,

                TransactionType =
                    "StockIn",

                Remarks =
                    string.IsNullOrWhiteSpace(
                        dto.Remarks)
                        ? null
                        : dto.Remarks.Trim(),

                CreatedDate =
                    DateTime.UtcNow
            };

            _repository.AddTransaction(
                transaction);

            await _repository.SaveChangesAsync();

            return MapInventory(product);
        }

        // =========================
        // STOCK ADJUSTMENT
        // =========================

        public async Task<InventoryResponseDto>
            AdjustStockAsync(
                StockAdjustmentDto dto)
        {
            if (dto.ProductId <= 0)
            {
                throw new Exception(
                    "Invalid product ID.");
            }

            if (dto.Quantity == 0)
            {
                throw new Exception(
                    "Adjustment quantity cannot be zero.");
            }

            var product =
                await _repository.GetProductByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            // Prevent negative stock
            if (product.Stock + dto.Quantity < 0)
            {
                throw new Exception(
                    "Stock cannot be negative.");
            }

            // Apply adjustment
            product.Stock += dto.Quantity;

            var transaction = new InventoryTransaction
            {
                ProductId =
                    product.ProductId,

                Quantity =
                    dto.Quantity,

                TransactionType =
                    "Adjustment",

                Remarks =
                    string.IsNullOrWhiteSpace(
                        dto.Remarks)
                        ? null
                        : dto.Remarks.Trim(),

                CreatedDate =
                    DateTime.UtcNow
            };

            _repository.AddTransaction(
                transaction);

            await _repository.SaveChangesAsync();

            return MapInventory(product);
        }

        // =========================
        // MAP INVENTORY
        // =========================

        private static InventoryResponseDto
            MapInventory(Product product)
        {
            return new InventoryResponseDto
            {
                ProductId =
                    product.ProductId,

                ProductName =
                    product.ProductName,

                SKU =
                    product.SKU,

                Stock =
                    product.Stock,

                ReorderLevel =
                    product.ReorderLevel,

                IsLowStock =
                    product.Stock > 0 &&
                    product.Stock <=
                    product.ReorderLevel,

                IsOutOfStock =
                    product.Stock == 0
            };
        }
    }
}