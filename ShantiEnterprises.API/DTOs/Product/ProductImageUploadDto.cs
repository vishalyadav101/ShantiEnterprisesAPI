using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Product
{
    public class ProductImageUploadDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;

        public bool IsPrimary { get; set; }
    }
}