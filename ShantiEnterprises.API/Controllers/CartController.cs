using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Cart;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var cart =
                await _service.GetCartAsync(userId);

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(
            AddToCartDto dto)
        {
            try
            {
                var userId = GetUserId();

                var cart =
                    await _service.AddToCartAsync(
                        userId,
                        dto);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateCartItem(
            int cartItemId,
            UpdateCartItemDto dto)
        {
            try
            {
                var userId = GetUserId();

                var cart =
                    await _service.UpdateCartItemAsync(
                        userId,
                        cartItemId,
                        dto);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveCartItem(
            int cartItemId)
        {
            var userId = GetUserId();

            var result =
                await _service.RemoveCartItemAsync(
                    userId,
                    cartItemId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Cart item not found."
                });
            }

            return Ok(new
            {
                message = "Item removed from cart."
            });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            await _service.ClearCartAsync(userId);

            return Ok(new
            {
                message = "Cart cleared successfully."
            });
        }

        private int GetUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return id;
        }
    }
}