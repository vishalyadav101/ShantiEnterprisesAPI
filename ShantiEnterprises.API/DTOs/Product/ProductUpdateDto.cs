using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal MRP { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal WholesalePrice { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, 100)]
        public decimal GSTPercentage { get; set; }

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}