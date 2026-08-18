using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public ReviewRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET BY PRODUCT
        // ==========================================

        public async Task<List<Review>> GetByProductIdAsync(
            int productId)
        {
            return await _context.Reviews
                .Include(x => x.User)
                .Where(x =>
                    x.ProductId == productId &&
                    x.IsActive &&
                    x.IsApproved)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<Review?> GetByIdAsync(
            int reviewId)
        {
            return await _context.Reviews
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.ReviewId == reviewId);
        }

        // ==========================================
        // GET BY USER + PRODUCT
        // ==========================================

        public async Task<Review?> GetByUserAndProductAsync(
            int userId,
            int productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == productId);
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task<Review> CreateAsync(
            Review review)
        {
            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            return review;
        }

        // ==========================================
        // UPDATE
        // ==========================================

        public async Task UpdateAsync(
            Review review)
        {
            _context.Reviews.Update(review);

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // DELETE
        // ==========================================

        public async Task DeleteAsync(
            Review review)
        {
            _context.Reviews.Remove(review);

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // SAVE CHANGES
        // ==========================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}