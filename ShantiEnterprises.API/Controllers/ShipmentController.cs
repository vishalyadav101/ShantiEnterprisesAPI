using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Shipment;
using ShantiEnterprises.API.Interfaces;
using System.Security.Claims;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(
            IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }


        // ==========================================
        // GET ALL SHIPMENTS
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var shipments =
                    await _shipmentService.GetAllAsync();

                return Ok(shipments);
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
        // GET SHIPMENT BY ID
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var shipment =
                    await _shipmentService.GetByIdAsync(id);

                return Ok(shipment);
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
        // GET SHIPMENT BY ORDER
        // ==========================================

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetByOrderId(
            int orderId)
        {
            try
            {
                var userIdClaim =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (!int.TryParse(
                    userIdClaim,
                    out int userId))
                {
                    return Unauthorized(new
                    {
                        message = "Invalid user identity."
                    });
                }

                var isAdmin =
                    User.IsInRole("Admin");

                var shipment =
                    await _shipmentService
                        .GetByOrderIdAsync(
                            orderId,
                            userId,
                            isAdmin);

                return Ok(shipment);
            }
            catch (UnauthorizedAccessException )
            {
                return Forbid();
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
        // CREATE SHIPMENT
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ShipmentCreateDto dto)
        {
            try
            {
                var shipment =
                    await _shipmentService
                        .CreateAsync(dto);

                return Ok(shipment);
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
        // UPDATE SHIPMENT
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ShipmentUpdateDto dto)
        {
            try
            {
                var shipment =
                    await _shipmentService
                        .UpdateAsync(id, dto);

                return Ok(shipment);
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
        // DELETE SHIPMENT
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                await _shipmentService
                    .DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "Shipment deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}