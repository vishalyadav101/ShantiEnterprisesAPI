namespace ShantiEnterprises.API.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string TransactionId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? RazorpayOrderId { get; set; }

        public string? RazorpayPaymentId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? Remarks { get; set; }
    }
}