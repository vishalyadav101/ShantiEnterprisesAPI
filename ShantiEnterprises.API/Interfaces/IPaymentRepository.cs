using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);

        Task<Payment?> GetByOrderIdAsync(
            int orderId);

        Task<Payment?> GetByIdAsync(
            int paymentId);

        Task UpdateAsync(
            Payment payment);
    }
}