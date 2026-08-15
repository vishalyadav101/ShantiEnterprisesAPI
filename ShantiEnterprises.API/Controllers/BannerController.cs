using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Banner;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _service;

        public BannerController(
            IBannerService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL BANNERS
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _service.GetAllAsync();

            return Ok(result);
        }

        // =========================
        // GET BANNER BY ID
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
                        message = "Banner not found."
                    });
            }

            return Ok(result);
        }

        // =========================
        // CREATE BANNER
        // =========================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] BannerCreateDto dto)
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
        // UPDATE BANNER
        // =========================

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] BannerUpdateDto dto)
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
                    "Banner not found.")
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
        // DELETE BANNER
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
                        message = "Banner not found."
                    });
            }

            return Ok(
                new
                {
                    message =
                        "Banner deleted successfully."
                });
        }
    }
}