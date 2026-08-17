using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public WishlistRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET WISHLIST BY USER
        // ==========================================

        public async Task<Wishlist?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Wishlists
                .Include(x => x.WishlistItems)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId);
        }

        // ==========================================
        // GET WISHLIST ITEM
        // ==========================================

        public async Task<WishlistItem?> GetItemAsync(
            int userId,
            int productId)
        {
            return await _context.WishlistItems
                .Include(x => x.Wishlist)
                .FirstOrDefaultAsync(x =>
                    x.Wishlist!.UserId == userId &&
                    x.ProductId == productId);
        }

        // ==========================================
        // CREATE WISHLIST
        // ==========================================

        public async Task<Wishlist> CreateAsync(
            Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);

            await _context.SaveChangesAsync();

            return wishlist;
        }

        // ==========================================
        // ADD ITEM
        // ==========================================

        public async Task<WishlistItem> AddItemAsync(
            WishlistItem item)
        {
            _context.WishlistItems.Add(item);

            await _context.SaveChangesAsync();

            return item;
        }

        // ==========================================
        // REMOVE ITEM
        // ==========================================

        public async Task RemoveItemAsync(
            WishlistItem item)
        {
            _context.WishlistItems.Remove(item);

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // CLEAR WISHLIST
        // ==========================================

        public async Task ClearAsync(
            Wishlist wishlist)
        {
            _context.WishlistItems.RemoveRange(
                wishlist.WishlistItems);

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // SAVE CHANGES
        // ==========================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}