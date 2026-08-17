namespace ShantiEnterprises.API.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        // =========================
        // NAVIGATION
        // =========================

        public User? User { get; set; }

        public ICollection<WishlistItem> WishlistItems { get; set; }
            = new List<WishlistItem>();
    }
}