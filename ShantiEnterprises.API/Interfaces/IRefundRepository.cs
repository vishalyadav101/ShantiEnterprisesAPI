using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IRefundRepository
    {
        // ==========================================
        // CREATE
        // ==========================================

        Task<Refund> CreateAsync(Refund refund);


        // ==========================================
        // GET BY ID
        // ==========================================

        Task<Refund?> GetByIdAsync(int refundId);


        // ==========================================
        // GET BY RETURN
        // ==========================================

        Task<Refund?> GetByReturnIdAsync(int returnId);


        // ==========================================
        // GET BY ORDER
        // ==========================================

        Task<Refund?> GetByOrderIdAsync(int orderId);


        // ==========================================
        // UPDATE
        // ==========================================

        Task UpdateAsync(Refund refund);
    }
}