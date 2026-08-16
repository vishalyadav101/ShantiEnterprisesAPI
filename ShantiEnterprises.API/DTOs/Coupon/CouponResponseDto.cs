namespace ShantiEnterprises.API.DTOs.Coupon
{
    public class CouponResponseDto
    {
        public int CouponId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }

        public decimal? MinimumOrderAmount { get; set; }

        public decimal? MaximumDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsCurrentlyValid { get; set; }
    }
}