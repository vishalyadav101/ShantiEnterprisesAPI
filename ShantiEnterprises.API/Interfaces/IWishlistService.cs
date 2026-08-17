using ShantiEnterprises.API.DTOs.Wishlist;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IWishlistService
    {
        Task<WishlistResponseDto> GetWishlistAsync(
            int userId);

        Task<WishlistResponseDto> AddItemAsync(
            int userId,
            AddWishlistItemDto dto);

        Task<WishlistResponseDto> RemoveItemAsync(
            int userId,
            int productId);

        Task ClearWishlistAsync(
            int userId);
    }
}