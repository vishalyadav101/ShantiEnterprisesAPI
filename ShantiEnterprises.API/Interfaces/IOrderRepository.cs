using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);

        Task<List<Order>> GetByUserIdAsync(
            int userId);

        Task<Order?> GetByIdAsync(
            int orderId,
            int userId);

        Task<Order?> GetByIdForAdminAsync(
            int orderId);

        Task UpdateAsync(Order order);

        Task ExecuteInTransactionAsync(
            Func<Task> action);
    }
}