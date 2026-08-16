using ShantiEnterprises.API.DTOs.Coupon;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICouponService
    {
        Task<List<CouponResponseDto>> GetAllAsync();

        Task<CouponResponseDto?> GetByIdAsync(
            int couponId);

        Task<CouponResponseDto> CreateAsync(
            CreateCouponDto dto);

        Task<CouponResponseDto?> UpdateAsync(
            int couponId,
            UpdateCouponDto dto);

        Task<bool> DeleteAsync(
            int couponId);

        Task<CouponResponseDto> ValidateCouponAsync(
            ValidateCouponDto dto);
    }
}