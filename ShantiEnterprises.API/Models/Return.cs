namespace ShantiEnterprises.API.Models
{
    public class Return
    {
        public int ReturnId { get; set; }

        public int OrderId { get; set; }

        public int OrderItemId { get; set; }

        public int UserId { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ReturnStatus { get; set; } = "Pending";

        public string? AdminComment { get; set; }

        public DateTime RequestedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? ApprovedDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }


        // ==========================================
        // NAVIGATION PROPERTIES
        // ==========================================

        public Order? Order { get; set; }

        public OrderItem? OrderItem { get; set; }

        public User? User { get; set; }

        public Refund? Refund { get; set; }
    }
}