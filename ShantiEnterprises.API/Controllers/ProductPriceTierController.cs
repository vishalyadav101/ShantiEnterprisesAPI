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

        // =========================================================
        // GET TIERS BY PRODUCT
        // GET: api/ProductPriceTier/product/1
        // =========================================================

        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProductId(
            int productId)
        {
            try
            {
                var tiers =
                    await _service.GetByProductIdAsync(
                        productId);

                return Ok(tiers);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =========================================================
        // CREATE PRICE TIER
        // POST: api/ProductPriceTier
        // =========================================================

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

        // =========================================================
        // UPDATE PRICE TIER
        // PUT: api/ProductPriceTier/1
        // =========================================================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            ProductPriceTierCreateDto dto)
        {
            try
            {
                var tier =
                    await _service.UpdateAsync(
                        id,
                        dto);

                if (tier == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Price tier not found."
                    });
                }

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

        // =========================================================
        // DELETE PRICE TIER
        // DELETE: api/ProductPriceTier/1
        // =========================================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var result =
                await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Price tier not found."
                });
            }

            return Ok(new
            {
                message =
                    "Price tier deleted successfully."
            });
        }
    }
}