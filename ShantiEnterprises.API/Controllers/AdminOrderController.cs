using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.AdminOrder;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _service;

        public AdminOrderController(
            IAdminOrderService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL ORDERS
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders =
                await _service.GetAllAsync();

            return Ok(orders);
        }

        // =========================
        // GET ORDER BY ID
        // =========================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var order =
                await _service.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            return Ok(order);
        }

        // =========================
        // UPDATE ORDER STATUS
        // =========================

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult>
            UpdateOrderStatus(
                int id,
                [FromBody]
                UpdateOrderStatusDto dto)
        {
            try
            {
                var result =
                    await _service.UpdateOrderStatusAsync(
                        id,
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

        // =========================
        // UPDATE PAYMENT STATUS
        // =========================

        [HttpPut("{id:int}/payment-status")]
        public async Task<IActionResult>
            UpdatePaymentStatus(
                int id,
                [FromBody]
                UpdatePaymentStatusDto dto)
        {
            try
            {
                var result =
                    await _service.UpdatePaymentStatusAsync(
                        id,
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
    }
}