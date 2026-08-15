using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public BannerRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL
        // =========================

        public async Task<List<Banner>> GetAllAsync()
        {
            return await _context.Banners
                .OrderBy(x => x.DisplayOrder)
                .ThenByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        // =========================
        // GET BY ID
        // =========================

        public async Task<Banner?> GetByIdAsync(
            int id)
        {
            return await _context.Banners
                .FirstOrDefaultAsync(
                    x => x.BannerId == id);
        }

        // =========================
        // CREATE
        // =========================

        public async Task<Banner> AddAsync(
            Banner banner)
        {
            _context.Banners.Add(banner);

            await _context.SaveChangesAsync();

            return banner;
        }

        // =========================
        // UPDATE
        // =========================

        public async Task UpdateAsync(
            Banner banner)
        {
            _context.Banners.Update(banner);

            await _context.SaveChangesAsync();
        }

        // =========================
        // DELETE
        // =========================

        public async Task<bool> DeleteAsync(
            int id)
        {
            var banner =
                await _context.Banners
                    .FirstOrDefaultAsync(
                        x => x.BannerId == id);

            if (banner == null)
            {
                return false;
            }

            _context.Banners.Remove(banner);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}