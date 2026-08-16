using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Coupon
{
    public class ValidateCouponDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal OrderAmount { get; set; }
    }
}