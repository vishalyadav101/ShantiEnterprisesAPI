using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);

        Task<Cart> CreateAsync(Cart cart);

        Task<CartItem?> GetItemAsync(
            int cartId,
            int productId);

        Task<CartItem?> GetItemByIdAsync(int cartItemId);

        Task<CartItem> AddItemAsync(CartItem item);

        Task UpdateItemAsync(CartItem item);

        Task RemoveItemAsync(CartItem item);

        Task ClearAsync(Cart cart);
    }
}