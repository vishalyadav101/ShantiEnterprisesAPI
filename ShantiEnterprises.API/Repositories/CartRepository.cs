using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public CartRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<CartItem?> GetItemAsync(
            int cartId,
            int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartId == cartId &&
                    x.ProductId == productId);
        }

        public async Task<CartItem?> GetItemByIdAsync(
            int cartItemId)
        {
            return await _context.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.CartItemId == cartItemId);
        }

        public async Task<CartItem> AddItemAsync(
            CartItem item)
        {
            _context.CartItems.Add(item);

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task UpdateItemAsync(
            CartItem item)
        {
            _context.CartItems.Update(item);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(
            CartItem item)
        {
            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }

        public async Task ClearAsync(Cart cart)
        {
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
        }
    }
}