using ShantiEnterprises.API.DTOs.Wishlist;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        // ==========================================
        // GET WISHLIST
        // ==========================================

        public async Task<WishlistResponseDto> GetWishlistAsync(
            int userId)
        {
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(
                    userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId
                };

                wishlist =
                    await _wishlistRepository.CreateAsync(
                        wishlist);
            }

            return MapToResponse(wishlist);
        }

        // ==========================================
        // ADD ITEM
        // ==========================================

        public async Task<WishlistResponseDto> AddItemAsync(
            int userId,
            AddWishlistItemDto dto)
        {
            // =========================
            // CHECK PRODUCT
            // =========================

            var product =
                await _productRepository.GetByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            if (!product.IsActive)
            {
                throw new Exception(
                    "This product is currently inactive.");
            }

            // =========================
            // GET / CREATE WISHLIST
            // =========================

            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(
                    userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId
                };

                wishlist =
                    await _wishlistRepository.CreateAsync(
                        wishlist);
            }

            // =========================
            // CHECK DUPLICATE
            // =========================

            var existingItem =
                await _wishlistRepository.GetItemAsync(
                    userId,
                    dto.ProductId);

            if (existingItem != null)
            {
                throw new Exception(
                    "Product is already in your wishlist.");
            }

            // =========================
            // ADD ITEM
            // =========================

            var item = new WishlistItem
            {
                WishlistId =
                    wishlist.WishlistId,

                ProductId =
                    dto.ProductId,

                AddedDate =
                    DateTime.UtcNow
            };

            await _wishlistRepository.AddItemAsync(
                item);

            // Reload wishlist
            wishlist =
                await _wishlistRepository.GetByUserIdAsync(
                    userId);

            return MapToResponse(
                wishlist!);
        }

        // ==========================================
        // REMOVE ITEM
        // ==========================================

        public async Task<WishlistResponseDto> RemoveItemAsync(
            int userId,
            int productId)
        {
            var item =
                await _wishlistRepository.GetItemAsync(
                    userId,
                    productId);

            if (item == null)
            {
                throw new Exception(
                    "Product is not in your wishlist.");
            }

            await _wishlistRepository.RemoveItemAsync(
                item);

            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(
                    userId);

            return MapToResponse(
                wishlist!);
        }

        // ==========================================
        // CLEAR WISHLIST
        // ==========================================

        public async Task ClearWishlistAsync(
            int userId)
        {
            var wishlist =
                await _wishlistRepository.GetByUserIdAsync(
                    userId);

            if (wishlist == null)
            {
                return;
            }

            await _wishlistRepository.ClearAsync(
                wishlist);
        }

        // ==========================================
        // RESPONSE MAPPING
        // ==========================================

        private static WishlistResponseDto MapToResponse(
            Wishlist wishlist)
        {
            return new WishlistResponseDto
            {
                WishlistId =
                    wishlist.WishlistId,

                UserId =
                    wishlist.UserId,

                CreatedDate =
                    wishlist.CreatedDate,

                TotalItems =
                    wishlist.WishlistItems.Count,

                Items =
                    wishlist.WishlistItems
                        .Where(x => x.Product != null)
                        .Select(x => new WishlistItemResponseDto
                        {
                            WishlistItemId =
                                x.WishlistItemId,

                            ProductId =
                                x.ProductId,

                            ProductName =
                                x.Product!.ProductName,

                            SKU =
                                x.Product.SKU,

                            ImageUrl =
                                x.Product.ImageUrl,

                            UnitPrice =
                                x.Product.WholesalePrice,

                            IsActive =
                                x.Product.IsActive,

                            AddedDate =
                                x.AddedDate
                        })
                        .ToList()
            };
        }
    }
}