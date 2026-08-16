namespace ShantiEnterprises.API.DTOs.Inventory
{
    public class InventoryResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public string SKU { get; set; }
            = string.Empty;

        public int Stock { get; set; }

        public int ReorderLevel { get; set; }

        public bool IsLowStock { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}