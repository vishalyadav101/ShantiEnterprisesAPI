using ShantiEnterprises.API.DTOs.Review;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }

        // ==========================================
        // GET REVIEWS BY PRODUCT
        // ==========================================

        public async Task<List<ReviewResponseDto>>
            GetByProductIdAsync(int productId)
        {
            var reviews =
                await _reviewRepository.GetByProductIdAsync(
                    productId);

            return reviews
                .Select(MapToResponse)
                .ToList();
        }

        // ==========================================
        // GET RATING SUMMARY BY PRODUCT
        // ==========================================

        public async Task<ReviewSummaryDto>
            GetSummaryByProductIdAsync(int productId)
        {
            var reviews =
                await _reviewRepository.GetByProductIdAsync(
                    productId);

            if (!reviews.Any())
            {
                return new ReviewSummaryDto
                {
                    ProductId = productId,
                    AverageRating = 0,
                    ReviewCount = 0
                };
            }

            var averageRating =
                reviews.Average(x => x.Rating);

            return new ReviewSummaryDto
            {
                ProductId = productId,

                AverageRating =
                    Math.Round(
                        (decimal)averageRating,
                        1),

                ReviewCount =
                    reviews.Count
            };
        }
        // ==========================================
        // GET REVIEW BY ID
        // ==========================================

        public async Task<ReviewResponseDto>
            GetByIdAsync(int reviewId)
        {
            var review =
                await _reviewRepository.GetByIdAsync(
                    reviewId);

            if (review == null)
            {
                throw new Exception(
                    "Review not found.");
            }

            return MapToResponse(review);
        }

        // ==========================================
        // CREATE REVIEW
        // ==========================================

        public async Task<ReviewResponseDto>
            CreateAsync(
                int userId,
                CreateReviewDto dto)
        {
            // =========================
            // CHECK PRODUCT
            // =========================

            var product =
                await _productRepository.GetByIdAsync(
                    dto.ProductId);

            if (product == null)
            {
                throw new Exception(
                    "Product not found.");
            }

            if (!product.IsActive)
            {
                throw new Exception(
                    "This product is currently inactive.");
            }

            // =========================
            // CHECK DUPLICATE
            // =========================

            var existingReview =
                await _reviewRepository
                    .GetByUserAndProductAsync(
                        userId,
                        dto.ProductId);

            if (existingReview != null)
            {
                throw new Exception(
                    "You have already reviewed this product.");
            }

            // =========================
            // CREATE REVIEW
            // =========================

            var review = new Review
            {
                ProductId = dto.ProductId,

                UserId = userId,

                Rating = dto.Rating,

                ReviewTitle = dto.ReviewTitle,

                ReviewComment = dto.ReviewComment,

                IsApproved = true,

                IsActive = true,

                CreatedDate = DateTime.UtcNow
            };

            await _reviewRepository.CreateAsync(
                review);

            // Reload with User
            review =
                await _reviewRepository.GetByIdAsync(
                    review.ReviewId);

            return MapToResponse(review!);
        }

        // ==========================================
        // UPDATE REVIEW
        // ==========================================

        public async Task<ReviewResponseDto>
            UpdateAsync(
                int userId,
                int reviewId,
                UpdateReviewDto dto)
        {
            var review =
                await _reviewRepository.GetByIdAsync(
                    reviewId);

            if (review == null)
            {
                throw new Exception(
                    "Review not found.");
            }

            // =========================
            // OWNERSHIP CHECK
            // =========================

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You can only update your own review.");
            }

            // =========================
            // UPDATE
            // =========================

            review.Rating =
                dto.Rating;

            review.ReviewTitle =
                dto.ReviewTitle;

            review.ReviewComment =
                dto.ReviewComment;

            review.UpdatedDate =
                DateTime.UtcNow;

            // Updated review can go through
            // approval again if moderation is added later.
            review.IsApproved = true;

            await _reviewRepository.UpdateAsync(
                review);

            return MapToResponse(review);
        }

        // ==========================================
        // DELETE REVIEW
        // ==========================================

        public async Task DeleteAsync(
            int userId,
            int reviewId)
        {
            var review =
                await _reviewRepository.GetByIdAsync(
                    reviewId);

            if (review == null)
            {
                throw new Exception(
                    "Review not found.");
            }

            // =========================
            // OWNERSHIP CHECK
            // =========================

            if (review.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You can only delete your own review.");
            }

            await _reviewRepository.DeleteAsync(
                review);
        }

        // ==========================================
        // MAP RESPONSE
        // ==========================================

        private static ReviewResponseDto
            MapToResponse(Review review)
        {
            return new ReviewResponseDto
            {
                ReviewId =
                    review.ReviewId,

                ProductId =
                    review.ProductId,

                UserId =
                    review.UserId,

                UserName =
                    review.User?.FullName
                    ?? string.Empty,

                Rating =
                    review.Rating,

                ReviewTitle =
                    review.ReviewTitle,

                ReviewComment =
                    review.ReviewComment,

                IsApproved =
                    review.IsApproved,

                IsActive =
                    review.IsActive,

                CreatedDate =
                    review.CreatedDate,

                UpdatedDate =
                    review.UpdatedDate
            };
        }
    }
}