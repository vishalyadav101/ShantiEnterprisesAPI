using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPriceTierController : ControllerBase
    {
        private readonly IProductPriceTierService _service;

        public ProductPriceTierController(
            IProductPriceTierService service)
        {
            _service = service;
        }

        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProductId(
            int productId)
        {
            var tiers =
                await _service.GetByProductIdAsync(productId);

            return Ok(tiers);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            ProductPriceTierCreateDto dto)
        {
            try
            {
                var tier =
                    await _service.CreateAsync(dto);

                return Ok(tier);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Price tier not found."
                });
            }

            return Ok(new
            {
                message = "Price tier deleted successfully."
            });
        }
    }
}