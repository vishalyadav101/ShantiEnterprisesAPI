namespace ShantiEnterprises.API.DTOs.Cart
{
    public class CartItemResponseDto
    {
        public int CartItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal GSTPercentage { get; set; }

        public decimal GSTAmount { get; set; }
    }
}