using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Payment;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =========================
        // CREATE PAYMENT
        // =========================

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            CreatePaymentDto dto)
        {
            try
            {
                var userId = GetUserId();

                var payment =
                    await _paymentService.CreatePaymentAsync(
                        userId,
                        dto);

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // GET PAYMENT BY ORDER
        // =========================

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentByOrder(
            int orderId)
        {
            try
            {
                var userId = GetUserId();

                var payment =
                    await _paymentService
                        .GetPaymentByOrderIdAsync(
                            userId,
                            orderId);

                if (payment == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Payment not found for this order."
                    });
                }

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================
        // USER ID
        // =========================

        private int GetUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    userId,
                    out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return id;
        }
    }
}