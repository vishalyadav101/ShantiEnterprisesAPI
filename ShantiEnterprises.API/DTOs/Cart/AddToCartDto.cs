using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Cart
{
    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}