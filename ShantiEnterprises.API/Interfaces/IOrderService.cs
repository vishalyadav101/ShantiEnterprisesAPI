using ShantiEnterprises.API.DTOs.Order;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(
            int userId,
            CreateOrderDto dto);

        Task<List<OrderResponseDto>> GetMyOrdersAsync(
            int userId);

        Task<OrderResponseDto?> GetMyOrderByIdAsync(
            int userId,
            int orderId);
    }
}