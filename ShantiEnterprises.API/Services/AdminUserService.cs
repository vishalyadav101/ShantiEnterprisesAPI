using ShantiEnterprises.API.DTOs.AdminUser;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _repository;

        public AdminUserService(
            IAdminUserRepository repository)
        {
            _repository = repository;
        }


        // ==========================================
        // GET ALL USERS
        // ==========================================

        public async Task<List<AdminUserResponseDto>> GetAllAsync()
        {
            var users =
                await _repository.GetAllAsync();

            return users
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // GET USER BY ID
        // ==========================================

        public async Task<AdminUserResponseDto?> GetByIdAsync(
            int userId)
        {
            var user =
                await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return MapToDto(user);
        }


        // ==========================================
        // UPDATE USER
        // ==========================================

        public async Task<AdminUserResponseDto> UpdateAsync(
            int userId,
            AdminUserUpdateDto dto)
        {
            var user =
                await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }


            // ======================================
            // VALIDATION
            // ======================================

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Mobile))
            {
                throw new ArgumentException(
                    "Mobile number is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                throw new ArgumentException(
                    "Role is required.");
            }


            // ======================================
            // UPDATE
            // ======================================

            user.FullName =
                dto.FullName.Trim();

            user.Email =
                dto.Email.Trim();

            user.Mobile =
                dto.Mobile.Trim();

            user.Role =
                dto.Role.Trim();


            await _repository.UpdateAsync(user);

            return MapToDto(user);
        }


        // ==========================================
        // UPDATE STATUS
        // ==========================================

        public async Task<AdminUserResponseDto> UpdateStatusAsync(
            int userId,
            AdminUserStatusDto dto)
        {
            var user =
                await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }


            user.IsActive =
                dto.IsActive;


            await _repository.UpdateAsync(user);

            return MapToDto(user);
        }


        // ==========================================
        // DELETE USER
        // ==========================================

        public async Task DeleteAsync(
            int userId)
        {
            var user =
                await _repository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }


            await _repository.DeleteAsync(user);
        }


        // ==========================================
        // MAP ENTITY → DTO
        // ==========================================

        private static AdminUserResponseDto MapToDto(
            User user)
        {
            return new AdminUserResponseDto
            {
                UserId = user.UserId,

                FullName = user.FullName,

                Email = user.Email,

                Mobile = user.Mobile,

                Role = user.Role,

                IsActive = user.IsActive,

                CreatedDate = user.CreatedDate
            };
        }
    }
}