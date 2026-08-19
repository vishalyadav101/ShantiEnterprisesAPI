using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReturnRepository
    {
        // ==========================================
        // CREATE
        // ==========================================

        Task<Return> CreateAsync(
            Return returnRequest);


        // ==========================================
        // GET ALL
        // ==========================================

        Task<List<Return>> GetAllAsync();


        // ==========================================
        // GET BY ID
        // ==========================================

        Task<Return?> GetByIdAsync(
            int returnId);


        // ==========================================
        // GET BY USER
        // ==========================================

        Task<List<Return>> GetByUserIdAsync(
            int userId);


        // ==========================================
        // GET ORDER FOR RETURN
        // ==========================================

        Task<Order?> GetOrderForReturnAsync(
            int orderId);


        // ==========================================
        // UPDATE
        // ==========================================

        Task UpdateAsync(
            Return returnRequest);


        // ==========================================
        // DELETE
        // ==========================================

        Task DeleteAsync(
            Return returnRequest);
    }
}