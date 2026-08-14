using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Address;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _service;

        public AddressController(
            IAddressService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var addresses =
                await _service.GetAllAsync(userId);

            return Ok(addresses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();

            var address =
                await _service.GetByIdAsync(
                    userId,
                    id);

            if (address == null)
            {
                return NotFound(new
                {
                    message = "Address not found."
                });
            }

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            AddressCreateDto dto)
        {
            var userId = GetUserId();

            var address =
                await _service.CreateAsync(
                    userId,
                    dto);

            return Ok(address);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            AddressUpdateDto dto)
        {
            var userId = GetUserId();

            var address =
                await _service.UpdateAsync(
                    userId,
                    id,
                    dto);

            if (address == null)
            {
                return NotFound(new
                {
                    message = "Address not found."
                });
            }

            return Ok(address);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var result =
                await _service.DeleteAsync(
                    userId,
                    id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Address not found."
                });
            }

            return Ok(new
            {
                message = "Address deleted successfully."
            });
        }

        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = GetUserId();

            var result =
                await _service.SetDefaultAsync(
                    userId,
                    id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Address not found."
                });
            }

            return Ok(new
            {
                message =
                    "Default address updated successfully."
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