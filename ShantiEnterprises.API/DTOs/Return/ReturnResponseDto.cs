namespace ShantiEnterprises.API.DTOs.Return
{
    public class ReturnResponseDto
    {
        public int ReturnId { get; set; }

        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int OrderItemId { get; set; }

        public int UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal RefundAmount { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ReturnStatus { get; set; } = string.Empty;

        public string? AdminComment { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public RefundResponseDto? Refund { get; set; }
    }
}