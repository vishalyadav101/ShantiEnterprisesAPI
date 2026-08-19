using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Return;
using ShantiEnterprises.API.Interfaces;
using System.Security.Claims;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _returnService;
        private readonly IRefundService _refundService;

        public ReturnController(
            IReturnService returnService,
            IRefundService refundService)
        {
            _returnService = returnService;
            _refundService = refundService;
        }


        // ==========================================
        // GET ALL RETURNS
        // ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var returns =
                    await _returnService.GetAllAsync();

                return Ok(returns);
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
        // GET MY RETURNS
        // CUSTOMER
        // ==========================================

        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReturns()
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

                var returns =
                    await _returnService
                        .GetByUserIdAsync(userId);

                return Ok(returns);
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
        // GET RETURN BY ID
        // ADMIN / CUSTOMER
        // ==========================================

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
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

                var returnRequest =
                    await _returnService
                        .GetByIdAsync(
                            id,
                            userId,
                            isAdmin);

                return Ok(returnRequest);
            }
            catch (UnauthorizedAccessException)
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
        // CREATE RETURN
        // CUSTOMER
        // ==========================================

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ReturnCreateDto dto)
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

                var returnRequest =
                    await _returnService
                        .CreateAsync(
                            userId,
                            dto);

                return Ok(returnRequest);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
        // UPDATE RETURN STATUS
        // ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] ReturnUpdateDto dto)
        {
            try
            {
                var returnRequest =
                    await _returnService
                        .UpdateStatusAsync(
                            id,
                            dto);

                return Ok(returnRequest);
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
        // DELETE RETURN
        // ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                await _returnService
                    .DeleteAsync(id);

                return Ok(new
                {
                    message =
                        "Return request deleted successfully."
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


        // ==========================================
        // CREATE REFUND
        // ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/refund")]
        public async Task<IActionResult> CreateRefund(
            int id)
        {
            try
            {
                var refund =
                    await _refundService
                        .CreateRefundAsync(id);

                return Ok(refund);
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
        // UPDATE REFUND STATUS
        // ADMIN
        // ==========================================

        [Authorize(Roles = "Admin")]
        [HttpPut("refund/{id:int}/status")]
        public async Task<IActionResult> UpdateRefundStatus(
            int id,
            [FromBody] RefundStatusUpdateDto dto)
        {
            try
            {
                var refund =
                    await _refundService
                        .UpdateStatusAsync(
                            id,
                            dto);

                return Ok(refund);
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
        // GET REFUND BY RETURN
        // ADMIN / CUSTOMER
        // ==========================================

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("{id:int}/refund")]
        public async Task<IActionResult> GetRefundByReturn(
            int id)
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

                var refund =
                    await _refundService
                        .GetByReturnIdAsync(
                            id,
                            userId,
                            isAdmin);

                return Ok(refund);
            }
            catch (UnauthorizedAccessException)
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
    }
}