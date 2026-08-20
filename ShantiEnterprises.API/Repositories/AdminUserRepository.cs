using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public AdminUserRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // GET ALL USERS
        // ==========================================

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET USER BY ID
        // ==========================================

        public async Task<User?> GetByIdAsync(
            int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.UserId == userId);
        }


        // ==========================================
        // UPDATE USER
        // ==========================================

        public async Task UpdateAsync(
            User user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // DELETE USER
        // ==========================================

        public async Task DeleteAsync(
            User user)
        {
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}