namespace ShantiEnterprises.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<Address> Addresses { get; set; }
    = new List<Address>();

        public ICollection<Cart> Carts { get; set; }
            = new List<Cart>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        public ICollection<BulkEnquiry> BulkEnquiries { get; set; }
            = new List<BulkEnquiry>();
    }
}