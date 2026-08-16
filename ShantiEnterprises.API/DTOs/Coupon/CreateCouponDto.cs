using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "Percentage";

        [Range(0.01, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaximumDiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? UsageLimit { get; set; }

        public bool IsActive { get; set; } = true;
    }
}