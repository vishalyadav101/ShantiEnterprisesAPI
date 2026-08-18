using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Review;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(
            IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ==========================================
        // GET REVIEWS BY PRODUCT
        // ==========================================

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProduct(
            int productId)
        {
            var reviews =
                await _reviewService.GetByProductIdAsync(
                    productId);

            return Ok(reviews);
        }

        // ==========================================
        // GET RATING SUMMARY BY PRODUCT
        // ==========================================

        [HttpGet("product/{productId}/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSummary(
            int productId)
        {
            var summary =
                await _reviewService
                    .GetSummaryByProductIdAsync(productId);

            return Ok(summary);
        }

        // ==========================================
        // GET REVIEW BY ID
        // ==========================================

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var review =
                    await _reviewService.GetByIdAsync(id);

                return Ok(review);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // ==========================================
        // CREATE REVIEW
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateReviewDto dto)
        {
            try
            {
                var userId = GetUserId();

                var review =
                    await _reviewService.CreateAsync(
                        userId,
                        dto);

                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ==========================================
        // UPDATE REVIEW
        // ==========================================

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateReviewDto dto)
        {
            try
            {
                var userId = GetUserId();

                var review =
                    await _reviewService.UpdateAsync(
                        userId,
                        id,
                        dto);

                return Ok(review);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ==========================================
        // DELETE REVIEW
        // ==========================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                var userId = GetUserId();

                await _reviewService.DeleteAsync(
                    userId,
                    id);

                return Ok(new
                {
                    message =
                        "Review deleted successfully."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ==========================================
        // GET USER ID FROM JWT
        // ==========================================

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException(
                    "User ID not found in token.");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}