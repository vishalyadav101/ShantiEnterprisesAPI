using ShantiEnterprises.API.DTOs.Review;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReviewService
    {
        // ==========================================
        // GET ALL REVIEWS
        // ADMIN
        // ==========================================

        Task<List<ReviewResponseDto>> GetAllAsync();

        // ==========================================
        // GET REVIEWS BY PRODUCT
        // ==========================================

        Task<List<ReviewResponseDto>> GetByProductIdAsync(
            int productId);

        // ==========================================
        // GET REVIEW BY ID
        // ==========================================

        Task<ReviewResponseDto> GetByIdAsync(
            int reviewId);

        // ==========================================
        // CREATE REVIEW
        // ==========================================

        Task<ReviewResponseDto> CreateAsync(
            int userId,
            CreateReviewDto dto);

        // ==========================================
        // UPDATE REVIEW
        // ==========================================

        Task<ReviewResponseDto> UpdateAsync(
            int userId,
            int reviewId,
            UpdateReviewDto dto);

        // ==========================================
        // DELETE REVIEW
        // ==========================================

        Task DeleteAsync(
            int userId,
            int reviewId);

        // ==========================================
        // GET RATING SUMMARY BY PRODUCT
        // ==========================================

        Task<ReviewSummaryDto> GetSummaryByProductIdAsync(
            int productId);
    }
}