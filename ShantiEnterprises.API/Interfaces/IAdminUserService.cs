using ShantiEnterprises.API.DTOs.AdminUser;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAdminUserService
    {
        // ==========================================
        // GET ALL USERS
        // ADMIN
        // ==========================================

        Task<List<AdminUserResponseDto>> GetAllAsync();


        // ==========================================
        // GET USER BY ID
        // ADMIN
        // ==========================================

        Task<AdminUserResponseDto?> GetByIdAsync(
            int userId);


        // ==========================================
        // UPDATE USER
        // ADMIN
        // ==========================================

        Task<AdminUserResponseDto> UpdateAsync(
            int userId,
            AdminUserUpdateDto dto);


        // ==========================================
        // UPDATE USER STATUS
        // ADMIN
        // ==========================================

        Task<AdminUserResponseDto> UpdateStatusAsync(
            int userId,
            AdminUserStatusDto dto);


        // ==========================================
        // DELETE USER
        // ADMIN
        // ==========================================

        Task DeleteAsync(
            int userId);
    }
}