namespace ShantiEnterprises.API.DTOs.AdminUser
{
    public class AdminUserResponseDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}