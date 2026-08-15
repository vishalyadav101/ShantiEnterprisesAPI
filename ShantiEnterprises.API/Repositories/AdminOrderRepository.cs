using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class AdminOrderRepository : IAdminOrderRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public AdminOrderRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(
            int orderId)
        {
            return await _context.Orders
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.OrderId == orderId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
        }
    }
}