namespace ShantiEnterprises.API.DTOs.AdminUser
{
    public class AdminUserUpdateDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}