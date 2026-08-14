using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Address
{
    public class AddressCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^[0-9]{6}$",
            ErrorMessage = "Pincode must be 6 digits.")]
        public string Pincode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Country { get; set; } = "India";

        [Required]
        [RegularExpression(
            "^(Home|Office|Other)$",
            ErrorMessage =
                "Address type must be Home, Office or Other.")]
        public string AddressType { get; set; } = "Home";

        public bool IsDefault { get; set; }
    }
}