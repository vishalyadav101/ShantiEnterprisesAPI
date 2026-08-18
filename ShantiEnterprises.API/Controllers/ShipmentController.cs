using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Shipment;
using ShantiEnterprises.API.Interfaces;

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

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetByOrderId(
            int orderId)
        {
            try
            {
                var shipment =
                    await _shipmentService
                        .GetByOrderIdAsync(orderId);

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
        // CREATE SHIPMENT
        // ==========================================

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