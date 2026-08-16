using ShantiEnterprises.API.DTOs.Payment;


namespace ShantiEnterprises.API.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(
            int userId,
            CreatePaymentDto dto);

        Task<PaymentResponseDto?> GetPaymentByOrderIdAsync(
            int userId,
            int orderId);

        Task<PaymentResponseDto> CreateRazorpayOrderAsync(
            int userId,
            int orderId);

        Task<PaymentResponseDto> VerifyRazorpayPaymentAsync(
            int userId,
            PaymentVerifyDto dto);
    }
}