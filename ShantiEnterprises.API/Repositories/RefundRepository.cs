using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class RefundRepository : IRefundRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public RefundRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // CREATE
        // ==========================================

        public async Task<Refund> CreateAsync(
            Refund refund)
        {
            _context.Refunds.Add(refund);

            await _context.SaveChangesAsync();

            return refund;
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<Refund?> GetByIdAsync(
            int refundId)
        {
            return await _context.Refunds

                .Include(x => x.Return)

                .Include(x => x.Order)

                .Include(x => x.Payment)

                .FirstOrDefaultAsync(
                    x => x.RefundId == refundId);
        }


        // ==========================================
        // GET BY RETURN
        // ==========================================

        public async Task<Refund?> GetByReturnIdAsync(
            int returnId)
        {
            return await _context.Refunds

                .Include(x => x.Return)

                .Include(x => x.Order)

                .Include(x => x.Payment)

                .FirstOrDefaultAsync(
                    x => x.ReturnId == returnId);
        }


        // ==========================================
        // GET BY ORDER
        // ==========================================

        public async Task<Refund?> GetByOrderIdAsync(
            int orderId)
        {
            return await _context.Refunds

                .Include(x => x.Return)

                .Include(x => x.Order)

                .Include(x => x.Payment)

                .FirstOrDefaultAsync(
                    x => x.OrderId == orderId);
        }


        // ==========================================
        // UPDATE
        // ==========================================

        public async Task UpdateAsync(
            Refund refund)
        {
            _context.Refunds.Update(refund);

            await _context.SaveChangesAsync();
        }
    }
}