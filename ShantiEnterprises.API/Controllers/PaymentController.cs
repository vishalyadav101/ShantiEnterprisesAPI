using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Payment;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =========================================================
        // CREATE PAYMENT
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody] CreatePaymentDto dto)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _paymentService.CreatePaymentAsync(
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

        // =========================================================
        // CREATE RAZORPAY ORDER
        // =========================================================

        [HttpPost("razorpay/create")]
        public async Task<IActionResult> CreateRazorpayOrder(
            [FromQuery] int orderId)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _paymentService.CreateRazorpayOrderAsync(
                        userId,
                        orderId);

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

        // =========================================================
        // VERIFY RAZORPAY PAYMENT
        // =========================================================

        [HttpPost("razorpay/verify")]
        public async Task<IActionResult> VerifyRazorpayPayment(
            [FromBody] PaymentVerifyDto dto)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _paymentService
                        .VerifyRazorpayPaymentAsync(
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

        // =========================================================
        // GET PAYMENT BY ORDER
        // =========================================================

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrder(
            int orderId)
        {
            try
            {
                var userId = GetUserId();

                var result =
                    await _paymentService
                        .GetPaymentByOrderIdAsync(
                            userId,
                            orderId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Payment not found."
                    });
                }

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

        // =========================================================
        // GET USER ID FROM JWT
        // =========================================================

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new Exception(
                    "User ID not found in token.");
            }

            if (!int.TryParse(
                    userIdClaim.Value,
                    out var userId))
            {
                throw new Exception(
                    "Invalid user ID in token.");
            }

            return userId;
        }
    }
}