using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface ICouponRepository
    {
        Task<List<Coupon>> GetAllAsync();

        Task<Coupon?> GetByIdAsync(
            int couponId);

        Task<Coupon?> GetByCodeAsync(
            string code);

        Task<Coupon> CreateAsync(
            Coupon coupon);

        Task UpdateAsync(
            Coupon coupon);
        void Update(Coupon coupon);

        Task DeleteAsync(
            Coupon coupon);
    }
}