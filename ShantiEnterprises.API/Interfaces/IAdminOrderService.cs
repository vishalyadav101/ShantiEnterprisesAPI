using ShantiEnterprises.API.DTOs.AdminOrder;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAdminOrderService
    {
        Task<List<AdminOrderResponseDto>> GetAllAsync();

        Task<AdminOrderResponseDto?>
            GetByIdAsync(int orderId);

        Task<AdminOrderResponseDto>
            UpdateOrderStatusAsync(
                int orderId,
                UpdateOrderStatusDto dto);

        Task<AdminOrderResponseDto>
            UpdatePaymentStatusAsync(
                int orderId,
                UpdatePaymentStatusDto dto);
    }
}