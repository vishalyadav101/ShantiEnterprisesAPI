namespace ShantiEnterprises.API.DTOs.Order
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        // Shipping Address
        public string ShippingFullName { get; set; } = string.Empty;

        public string ShippingMobile { get; set; } = string.Empty;

        public string ShippingAddressLine1 { get; set; } = string.Empty;

        public string? ShippingAddressLine2 { get; set; }

        public string ShippingCity { get; set; } = string.Empty;

        public string ShippingState { get; set; } = string.Empty;

        public string ShippingPincode { get; set; } = string.Empty;

        public string ShippingCountry { get; set; } = string.Empty;

        // =========================
        // AMOUNTS
        // =========================

        public decimal Subtotal { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal CouponDiscount { get; set; }

        public string? CouponCode { get; set; }

        public decimal GrandTotal { get; set; }

        // Status
        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public List<OrderItemResponseDto> Items { get; set; }
            = new();
    }
}