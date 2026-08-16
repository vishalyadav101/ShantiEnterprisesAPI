using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Coupon;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(
            ICouponService couponService)
        {
            _couponService = couponService;
        }

        // ==========================================
        // GET ALL COUPONS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coupons =
                await _couponService.GetAllAsync();

            return Ok(coupons);
        }

        // ==========================================
        // GET COUPON BY ID
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var coupon =
                await _couponService.GetByIdAsync(id);

            if (coupon == null)
            {
                return NotFound(new
                {
                    message = "Coupon not found."
                });
            }

            return Ok(coupon);
        }

        // ==========================================
        // CREATE COUPON
        // ==========================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            CreateCouponDto dto)
        {
            try
            {
                var coupon =
                    await _couponService.CreateAsync(dto);

                return Ok(coupon);
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
        // UPDATE COUPON
        // ==========================================

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCouponDto dto)
        {
            try
            {
                var coupon =
                    await _couponService.UpdateAsync(
                        id,
                        dto);

                if (coupon == null)
                {
                    return NotFound(new
                    {
                        message = "Coupon not found."
                    });
                }

                return Ok(coupon);
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
        // DELETE COUPON
        // ==========================================

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var deleted =
                await _couponService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Coupon not found."
                });
            }

            return Ok(new
            {
                message = "Coupon deleted successfully."
            });
        }

        // ==========================================
        // VALIDATE COUPON
        // ==========================================

        [HttpPost("validate")]
        public async Task<IActionResult> Validate(
            ValidateCouponDto dto)
        {
            try
            {
                var coupon =
                    await _couponService.ValidateCouponAsync(
                        dto);

                return Ok(new
                {
                    message = "Coupon is valid.",
                    coupon
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}