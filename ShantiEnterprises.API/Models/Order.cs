namespace ShantiEnterprises.API.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        public string OrderNumber { get; set; }
            = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal GrandTotal { get; set; }

        public string OrderStatus { get; set; }
            = "Pending";

        public string PaymentStatus { get; set; }
            = "Pending";

        // =========================
        // SHIPPING ADDRESS SNAPSHOT
        // =========================

        public string ShippingFullName { get; set; }
            = string.Empty;

        public string ShippingMobile { get; set; }
            = string.Empty;

        public string ShippingAddressLine1 { get; set; }
            = string.Empty;

        public string? ShippingAddressLine2 { get; set; }

        public string ShippingCity { get; set; }
            = string.Empty;

        public string ShippingState { get; set; }
            = string.Empty;

        public string ShippingPincode { get; set; }
            = string.Empty;

        public string ShippingCountry { get; set; }
            = string.Empty;

        // =========================
        // NAVIGATION
        // =========================

        public User? User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}