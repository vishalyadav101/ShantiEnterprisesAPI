using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public int AddressId { get; set; }

        public string? CouponCode { get; set; }
    }
}