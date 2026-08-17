namespace ShantiEnterprises.API.DTOs.Wishlist
{
    public class WishlistItemResponseDto
    {
        public int WishlistItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime AddedDate { get; set; }
    }
}