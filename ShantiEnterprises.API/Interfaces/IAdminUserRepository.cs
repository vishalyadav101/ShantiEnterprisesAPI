using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAdminUserRepository
    {
        // ==========================================
        // GET ALL USERS
        // ==========================================

        Task<List<User>> GetAllAsync();


        // ==========================================
        // GET USER BY ID
        // ==========================================

        Task<User?> GetByIdAsync(
            int userId);


        // ==========================================
        // UPDATE USER
        // ==========================================

        Task UpdateAsync(
            User user);


        // ==========================================
        // DELETE USER
        // ==========================================

        Task DeleteAsync(
            User user);
    }
}