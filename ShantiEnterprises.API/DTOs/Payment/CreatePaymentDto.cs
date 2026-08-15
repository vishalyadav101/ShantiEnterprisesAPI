using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}