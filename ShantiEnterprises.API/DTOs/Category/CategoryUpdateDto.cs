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

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}