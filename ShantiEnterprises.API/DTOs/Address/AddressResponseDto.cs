namespace ShantiEnterprises.API.DTOs.Address
{
    public class AddressResponseDto
    {
        public int AddressId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string AddressType { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}