namespace ShantiEnterprises.API.DTOs.AdminOrder
{
    public class AdminOrderItemResponseDto
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal GSTPercentage { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal TotalPrice { get; set; }
    }
}