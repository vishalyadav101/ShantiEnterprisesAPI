using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReviewRepository
    {
        // ==========================================
        // GET ALL REVIEWS
        // ADMIN
        // ==========================================

        Task<List<Review>> GetAllAsync();

        // ==========================================
        // GET REVIEWS BY PRODUCT
        // ==========================================

        Task<List<Review>> GetByProductIdAsync(
            int productId);

        // ==========================================
        // GET REVIEW BY ID
        // ==========================================

        Task<Review?> GetByIdAsync(
            int reviewId);

        // ==========================================
        // GET BY USER + PRODUCT
        // ==========================================

        Task<Review?> GetByUserAndProductAsync(
            int userId,
            int productId);

        // ==========================================
        // CREATE
        // ==========================================

        Task<Review> CreateAsync(
            Review review);

        // ==========================================
        // UPDATE
        // ==========================================

        Task UpdateAsync(
            Review review);

        // ==========================================
        // DELETE
        // ==========================================

        Task DeleteAsync(
            Review review);

        // ==========================================
        // SAVE CHANGES
        // ==========================================

        Task SaveChangesAsync();
    }
}