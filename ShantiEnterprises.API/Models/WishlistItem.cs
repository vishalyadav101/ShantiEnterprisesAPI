namespace ShantiEnterprises.API.Models
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }

        public int WishlistId { get; set; }

        public int ProductId { get; set; }

        public DateTime AddedDate { get; set; }
            = DateTime.UtcNow;

        // =========================
        // NAVIGATION
        // =========================

        public Wishlist? Wishlist { get; set; }

        public Product? Product { get; set; }
    }
}