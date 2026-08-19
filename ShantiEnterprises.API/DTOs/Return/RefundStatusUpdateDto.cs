using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Return
{
    public class RefundStatusUpdateDto
    {
        [Required]
        public string RefundStatus { get; set; }
            = string.Empty;

        public string? RefundReference { get; set; }

        public string? FailureReason { get; set; }
    }
}