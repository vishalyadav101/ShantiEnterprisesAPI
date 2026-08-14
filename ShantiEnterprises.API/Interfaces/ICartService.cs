using ShantiEnterprises.API.DTOs.Cart;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);

        Task<CartResponseDto> AddToCartAsync(
            int userId,
            AddToCartDto dto);

        Task<CartResponseDto> UpdateCartItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto);

        Task<bool> RemoveCartItemAsync(
            int userId,
            int cartItemId);

        Task ClearCartAsync(int userId);
    }
}