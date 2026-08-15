using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Offer;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _service;

        public OfferController(
            IOfferService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL OFFERS
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _service.GetAllAsync();

            return Ok(result);
        }

        // =========================
        // GET OFFER BY ID
        // =========================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message = "Offer not found."
                    });
            }

            return Ok(result);
        }

        // =========================
        // CREATE OFFER
        // =========================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] OfferCreateDto dto)
        {
            try
            {
                var result =
                    await _service.CreateAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        // =========================
        // UPDATE OFFER
        // =========================

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] OfferUpdateDto dto)
        {
            try
            {
                var result =
                    await _service.UpdateAsync(
                        id,
                        dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message ==
                    "Offer not found.")
                {
                    return NotFound(
                        new
                        {
                            message = ex.Message
                        });
                }

                return BadRequest(
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        // =========================
        // DELETE OFFER
        // =========================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var deleted =
                await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(
                    new
                    {
                        message = "Offer not found."
                    });
            }

            return Ok(
                new
                {
                    message =
                        "Offer deleted successfully."
                });
        }
    }
}