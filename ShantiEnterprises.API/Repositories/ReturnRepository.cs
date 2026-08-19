using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ReturnRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task<Return> CreateAsync(
            Return returnRequest)
        {
            _context.Returns.Add(returnRequest);

            await _context.SaveChangesAsync();

            return returnRequest;
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<Return>> GetAllAsync()
        {
            return await _context.Returns

                .Include(x => x.Order)

                .Include(x => x.OrderItem)

                .Include(x => x.User)

                .Include(x => x.Refund)

                .OrderByDescending(x => x.CreatedDate)

                .ToListAsync();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<Return?> GetByIdAsync(
            int returnId)
        {
            return await _context.Returns

                .Include(x => x.Order)

                .Include(x => x.OrderItem)

                .Include(x => x.User)

                .Include(x => x.Refund)

                .FirstOrDefaultAsync(
                    x => x.ReturnId == returnId);
        }


        // ==========================================
        // GET BY USER
        // ==========================================

        public async Task<List<Return>> GetByUserIdAsync(
            int userId)
        {
            return await _context.Returns

                .Include(x => x.Order)

                .Include(x => x.OrderItem)

                .Include(x => x.User)

                .Include(x => x.Refund)

                .Where(x => x.UserId == userId)

                .OrderByDescending(x => x.CreatedDate)

                .ToListAsync();
        }

        // ==========================================
        // GET ORDER FOR RETURN
        // ==========================================

        public async Task<Order?> GetOrderForReturnAsync(
            int orderId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(
                    x => x.OrderId == orderId);
        }

        // ==========================================
        // UPDATE
        // ==========================================

        public async Task UpdateAsync(
            Return returnRequest)
        {
            _context.Returns.Update(returnRequest);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // DELETE
        // ==========================================

        public async Task DeleteAsync(
            Return returnRequest)
        {
            _context.Returns.Remove(returnRequest);

            await _context.SaveChangesAsync();
        }
    }
}