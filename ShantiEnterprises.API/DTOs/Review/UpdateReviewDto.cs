using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.Review
{
    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string? ReviewTitle { get; set; }

        [MaxLength(2000)]
        public string? ReviewComment { get; set; }
    }
}