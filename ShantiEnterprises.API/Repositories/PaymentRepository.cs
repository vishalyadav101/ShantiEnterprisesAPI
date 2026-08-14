using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public PaymentRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(
            Payment payment)
        {
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment?> GetByOrderIdAsync(
            int orderId)
        {
            return await _context.Payments
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x =>
                    x.OrderId == orderId);
        }

        public async Task<Payment?> GetByIdAsync(
            int paymentId)
        {
            return await _context.Payments
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x =>
                    x.PaymentId == paymentId);
        }
    }
}