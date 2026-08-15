using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.WebsiteSetting;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteSettingController : ControllerBase
    {
        private readonly IWebsiteSettingService _service;

        public WebsiteSettingController(
            IWebsiteSettingService service)
        {
            _service = service;
        }

        // =========================
        // GET SETTINGS
        // =========================

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result =
                await _service.GetAsync();

            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Website settings not found."
                    });
            }

            return Ok(result);
        }

        // =========================
        // CREATE / UPDATE SETTINGS
        // =========================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Save(
            [FromForm] WebsiteSettingCreateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                    dto.CompanyName))
            {
                return BadRequest(
                    new
                    {
                        message =
                            "Company name is required."
                    });
            }

            var result =
                await _service.SaveAsync(dto);

            return Ok(result);
        }
    }
}