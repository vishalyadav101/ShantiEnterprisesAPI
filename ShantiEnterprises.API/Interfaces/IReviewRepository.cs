using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetByProductIdAsync(
            int productId);

        Task<Review?> GetByIdAsync(
            int reviewId);

        Task<Review?> GetByUserAndProductAsync(
            int userId,
            int productId);

        Task<Review> CreateAsync(
            Review review);

        Task UpdateAsync(
            Review review);

        Task DeleteAsync(
            Review review);

        Task SaveChangesAsync();
    }
}