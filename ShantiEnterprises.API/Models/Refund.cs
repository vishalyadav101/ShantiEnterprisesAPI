namespace ShantiEnterprises.API.Models
{
    public class Refund
    {
        public int RefundId { get; set; }

        public int ReturnId { get; set; }

        public int OrderId { get; set; }

        public int PaymentId { get; set; }

        public decimal RefundAmount { get; set; }

        public string RefundStatus { get; set; } = "NotInitiated";

        public string? RefundReference { get; set; }

        public DateTime? RefundDate { get; set; }

        public string? FailureReason { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }


        // ==========================================
        // NAVIGATION PROPERTIES
        // ==========================================

        public Return? Return { get; set; }

        public Order? Order { get; set; }

        public Payment? Payment { get; set; }
    }
}