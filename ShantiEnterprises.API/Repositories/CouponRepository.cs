using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public CouponRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<Coupon?> GetByIdAsync(
            int couponId)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(x =>
                    x.CouponId == couponId);
        }

        // ==========================================
        // GET BY CODE
        // ==========================================

        public async Task<Coupon?> GetByCodeAsync(
            string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(x =>
                    x.Code == code);
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task<Coupon> CreateAsync(
            Coupon coupon)
        {
            _context.Coupons.Add(coupon);

            await _context.SaveChangesAsync();

            return coupon;
        }

        // ==========================================
        // UPDATE
        // ==========================================

        public async Task UpdateAsync(
            Coupon coupon)
        {
            _context.Coupons.Update(coupon);

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // DELETE
        // ==========================================

        public async Task DeleteAsync(
            Coupon coupon)
        {
            _context.Coupons.Remove(coupon);

            await _context.SaveChangesAsync();
        }
    }
}