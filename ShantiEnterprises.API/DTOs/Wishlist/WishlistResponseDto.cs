namespace ShantiEnterprises.API.DTOs.Wishlist
{
    public class WishlistResponseDto
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int TotalItems { get; set; }

        public List<WishlistItemResponseDto> Items { get; set; }
            = new();
    }
}