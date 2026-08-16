using ShantiEnterprises.API.DTOs.Coupon;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(
            ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<CouponResponseDto>> GetAllAsync()
        {
            var coupons =
                await _couponRepository.GetAllAsync();

            return coupons
                .Select(MapToResponse)
                .ToList();
        }

        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<CouponResponseDto?> GetByIdAsync(
            int couponId)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(
                    couponId);

            if (coupon == null)
            {
                return null;
            }

            return MapToResponse(coupon);
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task<CouponResponseDto> CreateAsync(
            CreateCouponDto dto)
        {
            // Validate discount type
            var discountType =
                dto.DiscountType.Trim();

            if (!discountType.Equals(
                    "Percentage",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !discountType.Equals(
                    "Fixed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid discount type. Use Percentage or Fixed.");
            }

            discountType =
                discountType.Equals(
                    "Percentage",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Percentage"
                    : "Fixed";

            // Validate code
            var code =
                dto.Code.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new Exception(
                    "Coupon code is required.");
            }

            // Check duplicate code
            var existingCoupon =
                await _couponRepository.GetByCodeAsync(
                    code);

            if (existingCoupon != null)
            {
                throw new Exception(
                    "Coupon code already exists.");
            }

            // Validate dates
            if (dto.EndDate <= dto.StartDate)
            {
                throw new Exception(
                    "End date must be greater than start date.");
            }

            // Validate percentage
            if (discountType == "Percentage" &&
                dto.DiscountValue > 100)
            {
                throw new Exception(
                    "Percentage discount cannot exceed 100.");
            }

            // Validate maximum discount
            if (discountType == "Fixed" &&
                dto.MaximumDiscountAmount.HasValue)
            {
                throw new Exception(
                    "Maximum discount amount is only applicable to Percentage coupons.");
            }

            var coupon = new Coupon
            {
                Code = code,

                Description =
                    dto.Description?.Trim()
                    ?? string.Empty,

                DiscountType =
                    discountType,

                DiscountValue =
                    dto.DiscountValue,

                MinimumOrderAmount =
                    dto.MinimumOrderAmount,

                MaximumDiscountAmount =
                    dto.MaximumDiscountAmount,

                StartDate =
                    dto.StartDate,

                EndDate =
                    dto.EndDate,

                UsageLimit =
                    dto.UsageLimit,

                UsedCount = 0,

                IsActive =
                    dto.IsActive,

                CreatedDate =
                    DateTime.UtcNow
            };

            var createdCoupon =
                await _couponRepository.CreateAsync(
                    coupon);

            return MapToResponse(createdCoupon);
        }

        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<CouponResponseDto?> UpdateAsync(
            int couponId,
            UpdateCouponDto dto)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(
                    couponId);

            if (coupon == null)
            {
                return null;
            }

            var discountType =
                dto.DiscountType.Trim();

            if (!discountType.Equals(
                    "Percentage",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !discountType.Equals(
                    "Fixed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid discount type. Use Percentage or Fixed.");
            }

            discountType =
                discountType.Equals(
                    "Percentage",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Percentage"
                    : "Fixed";

            var code =
                dto.Code.Trim().ToUpper();

            var existingCoupon =
                await _couponRepository.GetByCodeAsync(
                    code);

            if (existingCoupon != null &&
                existingCoupon.CouponId != couponId)
            {
                throw new Exception(
                    "Coupon code already exists.");
            }

            if (dto.EndDate <= dto.StartDate)
            {
                throw new Exception(
                    "End date must be greater than start date.");
            }

            if (discountType == "Percentage" &&
                dto.DiscountValue > 100)
            {
                throw new Exception(
                    "Percentage discount cannot exceed 100.");
            }

            if (discountType == "Fixed" &&
                dto.MaximumDiscountAmount.HasValue)
            {
                throw new Exception(
                    "Maximum discount amount is only applicable to Percentage coupons.");
            }

            coupon.Code =
                code;

            coupon.Description =
                dto.Description?.Trim()
                ?? string.Empty;

            coupon.DiscountType =
                discountType;

            coupon.DiscountValue =
                dto.DiscountValue;

            coupon.MinimumOrderAmount =
                dto.MinimumOrderAmount;

            coupon.MaximumDiscountAmount =
                dto.MaximumDiscountAmount;

            coupon.StartDate =
                dto.StartDate;

            coupon.EndDate =
                dto.EndDate;

            coupon.UsageLimit =
                dto.UsageLimit;

            coupon.IsActive =
                dto.IsActive;

            await _couponRepository.UpdateAsync(
                coupon);

            return MapToResponse(coupon);
        }

        // ==========================================
        // DELETE
        // ==========================================

        public async Task<bool> DeleteAsync(
            int couponId)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(
                    couponId);

            if (coupon == null)
            {
                return false;
            }

            await _couponRepository.DeleteAsync(
                coupon);

            return true;
        }

        // ==========================================
        // VALIDATE COUPON
        // ==========================================

        public async Task<CouponResponseDto> ValidateCouponAsync(
            ValidateCouponDto dto)
        {
            var code =
                dto.Code.Trim().ToUpper();

            var coupon =
                await _couponRepository.GetByCodeAsync(
                    code);

            if (coupon == null)
            {
                throw new Exception(
                    "Invalid coupon code.");
            }

            var now =
                DateTime.UtcNow;

            // Active check
            if (!coupon.IsActive)
            {
                throw new Exception(
                    "This coupon is inactive.");
            }

            // Start date
            if (now < coupon.StartDate)
            {
                throw new Exception(
                    "This coupon is not active yet.");
            }

            // End date
            if (now > coupon.EndDate)
            {
                throw new Exception(
                    "This coupon has expired.");
            }

            // Usage limit
            if (coupon.UsageLimit.HasValue &&
                coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                throw new Exception(
                    "This coupon usage limit has been reached.");
            }

            // Minimum order amount
            if (coupon.MinimumOrderAmount.HasValue &&
                dto.OrderAmount < coupon.MinimumOrderAmount.Value)
            {
                throw new Exception(
                    $"Minimum order amount should be {coupon.MinimumOrderAmount.Value:0.00}.");
            }

            return MapToResponse(coupon);
        }

        // ==========================================
        // RESPONSE MAPPING
        // ==========================================

        private static CouponResponseDto MapToResponse(
            Coupon coupon)
        {
            var now =
                DateTime.UtcNow;

            var isCurrentlyValid =
                coupon.IsActive
                &&
                now >= coupon.StartDate
                &&
                now <= coupon.EndDate
                &&
                (
                    !coupon.UsageLimit.HasValue
                    ||
                    coupon.UsedCount < coupon.UsageLimit.Value
                );

            return new CouponResponseDto
            {
                CouponId =
                    coupon.CouponId,

                Code =
                    coupon.Code,

                Description =
                    coupon.Description,

                DiscountType =
                    coupon.DiscountType,

                DiscountValue =
                    coupon.DiscountValue,

                MinimumOrderAmount =
                    coupon.MinimumOrderAmount,

                MaximumDiscountAmount =
                    coupon.MaximumDiscountAmount,

                StartDate =
                    coupon.StartDate,

                EndDate =
                    coupon.EndDate,

                UsageLimit =
                    coupon.UsageLimit,

                UsedCount =
                    coupon.UsedCount,

                IsActive =
                    coupon.IsActive,

                CreatedDate =
                    coupon.CreatedDate,

                IsCurrentlyValid =
                    isCurrentlyValid
            };
        }
    }
}