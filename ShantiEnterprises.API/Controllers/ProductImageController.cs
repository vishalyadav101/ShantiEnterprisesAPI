using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Product;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImageController(
            IProductImageService service)
        {
            _service = service;
        }


        // GET: api/ProductImage/product/1
        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProductId(
            int productId)
        {
            var images =
                await _service.GetByProductIdAsync(productId);

            return Ok(images);
        }


        // POST:
        // api/ProductImage/upload/1
        [HttpPost("upload/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
        int productId,
        [FromForm] ProductImageUploadDto dto)
        {
            try
            {
                var image =
                    await _service.UploadAsync(
                        productId,
                        dto);

                return Ok(image);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // DELETE:
        // api/ProductImage/1
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
                    message = "Product image not found."
                });
            }

            return Ok(new
            {
                message = "Product image deleted successfully."
            });
        }
    }
}