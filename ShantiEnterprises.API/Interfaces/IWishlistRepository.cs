using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByUserIdAsync(
            int userId);

        Task<WishlistItem?> GetItemAsync(
            int userId,
            int productId);

        Task<Wishlist> CreateAsync(
            Wishlist wishlist);

        Task<WishlistItem> AddItemAsync(
            WishlistItem item);

        Task RemoveItemAsync(
            WishlistItem item);

        Task ClearAsync(
            Wishlist wishlist);

        Task SaveChangesAsync();
    }
}