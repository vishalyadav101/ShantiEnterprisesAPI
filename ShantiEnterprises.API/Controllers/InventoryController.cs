using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Inventory;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(
            IInventoryService service)
        {
            _service = service;
        }

        // =========================
        // GET ALL INVENTORY
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _service.GetAllAsync();

            return Ok(result);
        }

        // =========================
        // GET INVENTORY BY PRODUCT
        // =========================

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(
            int productId)
        {
            var result =
                await _service.GetByProductIdAsync(
                    productId);

            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Product not found."
                    });
            }

            return Ok(result);
        }

        // =========================
        // GET TRANSACTION HISTORY
        // =========================

        [HttpGet("product/{productId}/transactions")]
        public async Task<IActionResult>
            GetTransactions(int productId)
        {
            try
            {
                var result =
                    await _service.GetTransactionsAsync(
                        productId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message ==
                    "Product not found.")
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
        // STOCK IN
        // =========================

        [HttpPost("stock-in")]
        public async Task<IActionResult> StockIn(
            [FromBody] StockInDto dto)
        {
            try
            {
                var result =
                    await _service.StockInAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message ==
                    "Product not found.")
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
        // STOCK ADJUSTMENT
        // =========================

        [HttpPost("adjust")]
        public async Task<IActionResult> AdjustStock(
            [FromBody] StockAdjustmentDto dto)
        {
            try
            {
                var result =
                    await _service.AdjustStockAsync(
                        dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message ==
                    "Product not found.")
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
    }
}