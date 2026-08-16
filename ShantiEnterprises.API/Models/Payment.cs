namespace ShantiEnterprises.API.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? RazorpayOrderId { get; set; }

        public string? RazorpayPaymentId { get; set; }

        public string? RazorpaySignature { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public DateTime? PaymentDate { get; set; }

        public string? Remarks { get; set; }

        public Order? Order { get; set; }
    }
}