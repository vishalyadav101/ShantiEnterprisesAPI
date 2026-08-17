using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShantiEnterprises.API.DTOs.Wishlist;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(
            IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // ==========================================
        // GET WISHLIST
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _wishlistService.GetWishlistAsync(
                        userId);

                return Ok(result);
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
        // ADD PRODUCT
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> AddItem(
            [FromBody] AddWishlistItemDto dto)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _wishlistService.AddItemAsync(
                        userId,
                        dto);

                return Ok(result);
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
        // REMOVE PRODUCT
        // ==========================================

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> RemoveItem(
            int productId)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _wishlistService.RemoveItemAsync(
                        userId,
                        productId);

                return Ok(result);
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
        // CLEAR WISHLIST
        // ==========================================

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            try
            {
                var userId = GetUserId();

                await _wishlistService.ClearWishlistAsync(
                    userId);

                return Ok(new
                {
                    message =
                        "Wishlist cleared successfully."
                });
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

            if (!int.TryParse(
                    userIdClaim.Value,
                    out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user ID in token.");
            }

            return userId;
        }
    }
}