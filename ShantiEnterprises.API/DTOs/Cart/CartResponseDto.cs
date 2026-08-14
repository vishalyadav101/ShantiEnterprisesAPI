namespace ShantiEnterprises.API.DTOs.Cart
{
    public class CartResponseDto
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public List<CartItemResponseDto> Items { get; set; }
            = new();

        public decimal Subtotal { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public int TotalItems { get; set; }
    }
}