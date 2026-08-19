namespace ShantiEnterprises.API.DTOs.Return
{
    public class RefundResponseDto
    {
        public int RefundId { get; set; }

        public int ReturnId { get; set; }

        public int OrderId { get; set; }

        public int PaymentId { get; set; }

        public decimal RefundAmount { get; set; }

        public string RefundStatus { get; set; } = string.Empty;

        public string? RefundReference { get; set; }

        public DateTime? RefundDate { get; set; }

        public string? FailureReason { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}