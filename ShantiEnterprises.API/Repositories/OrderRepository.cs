using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public OrderRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetByUserIdAsync(
            int userId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(
            int orderId,
            int userId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.OrderId == orderId &&
                    x.UserId == userId);
        }

        public async Task<Order?> GetByIdForAdminAsync(
         int orderId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.OrderId == orderId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
        }
        public async Task ExecuteInTransactionAsync(
    Func<Task> action)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await action();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}