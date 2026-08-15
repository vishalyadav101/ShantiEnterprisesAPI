namespace ShantiEnterprises.API.DTOs.AdminOrder
{
    public class AdminOrderResponseDto
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        public string ShippingFullName { get; set; } = string.Empty;

        public string ShippingMobile { get; set; } = string.Empty;

        public string ShippingAddressLine1 { get; set; } = string.Empty;

        public string? ShippingAddressLine2 { get; set; }

        public string ShippingCity { get; set; } = string.Empty;

        public string ShippingState { get; set; } = string.Empty;

        public string ShippingPincode { get; set; } = string.Empty;

        public string ShippingCountry { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal GrandTotal { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<AdminOrderItemResponseDto> Items { get; set; }
            = new();
    }
}