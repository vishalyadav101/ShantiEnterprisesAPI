using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.AdminOrder
{
    public class UpdatePaymentStatusDto
    {
        [Required]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}