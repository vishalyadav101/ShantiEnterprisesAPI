namespace ShantiEnterprises.API.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int? AddressId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal Subtotal { get; set; }

        public decimal GSTAmount { get; set; }

        public decimal ShippingAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public string Status { get; set; } = "Pending";

        public string PaymentMethod { get; set; } = "COD";

        public string PaymentStatus { get; set; } = "Pending";

        public string ShippingAddress { get; set; } = string.Empty;

        public User? User { get; set; }

        public Address? Address { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}