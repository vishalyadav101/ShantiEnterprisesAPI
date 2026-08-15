using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.AdminOrder
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string OrderStatus { get; set; } = string.Empty;
    }
}