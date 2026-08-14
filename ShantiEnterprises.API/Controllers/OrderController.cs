using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Order;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // =========================
        // CREATE ORDER
        // =========================

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            CreateOrderDto dto)
        {
            try
            {
                var userId = GetUserId();

                var order =
                    await _orderService.CreateOrderAsync(
                        userId,
                        dto);

                return Ok(order);
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
        // GET MY ORDERS
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var userId = GetUserId();

                var orders =
                    await _orderService.GetMyOrdersAsync(
                        userId);

                return Ok(orders);
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
        // GET MY ORDER BY ID
        // =========================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMyOrderById(
            int id)
        {
            try
            {
                var userId = GetUserId();

                var order =
                    await _orderService.GetMyOrderByIdAsync(
                        userId,
                        id);

                if (order == null)
                {
                    return NotFound(new
                    {
                        message = "Order not found."
                    });
                }

                return Ok(order);
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
        // GET USER ID FROM JWT
        // =========================

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