using ShantiEnterprises.API.DTOs.Return;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IRefundService
    {
        // ==========================================
        // CREATE REFUND
        // ADMIN
        // ==========================================

        Task<RefundResponseDto> CreateRefundAsync(
            int returnId);


        // ==========================================
        // GET REFUND BY ID
        // ==========================================

        Task<RefundResponseDto> GetByIdAsync(
            int refundId,
            int userId,
            bool isAdmin);


        // ==========================================
        // GET REFUND BY RETURN
        // ==========================================

        Task<RefundResponseDto> GetByReturnIdAsync(
            int returnId,
            int userId,
            bool isAdmin);


        // ==========================================
        // GET REFUND BY ORDER
        // ==========================================

        Task<RefundResponseDto?> GetByOrderIdAsync(
            int orderId,
            int userId,
            bool isAdmin);


        // ==========================================
        // UPDATE REFUND STATUS
        // ADMIN
        // ==========================================

        Task<RefundResponseDto> UpdateStatusAsync(
            int refundId,
            RefundStatusUpdateDto dto);
    }
}