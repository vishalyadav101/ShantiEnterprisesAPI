using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // New category image
        // Optional hai, isliye purani image ko preserve kar sakte hain
        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
    }
}