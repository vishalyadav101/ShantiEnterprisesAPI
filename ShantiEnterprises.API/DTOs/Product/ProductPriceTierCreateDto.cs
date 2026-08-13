using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductPriceTierCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int MinQuantity { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxQuantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}