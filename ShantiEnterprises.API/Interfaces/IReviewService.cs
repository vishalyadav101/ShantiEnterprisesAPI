using ShantiEnterprises.API.DTOs.Review;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponseDto>> GetByProductIdAsync(
            int productId);

        Task<ReviewResponseDto> GetByIdAsync(
            int reviewId);

        Task<ReviewResponseDto> CreateAsync(
            int userId,
            CreateReviewDto dto);

        Task<ReviewResponseDto> UpdateAsync(
            int userId,
            int reviewId,
            UpdateReviewDto dto);

        Task DeleteAsync(
            int userId,
            int reviewId);

        Task<ReviewSummaryDto> GetSummaryByProductIdAsync(
            int productId);
    }
}