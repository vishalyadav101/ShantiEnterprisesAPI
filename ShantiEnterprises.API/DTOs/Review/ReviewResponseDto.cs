namespace ShantiEnterprises.API.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? ReviewTitle { get; set; }

        public string? ReviewComment { get; set; }

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}