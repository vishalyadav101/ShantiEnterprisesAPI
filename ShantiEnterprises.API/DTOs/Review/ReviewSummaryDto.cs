namespace ShantiEnterprises.API.DTOs.Review
{
    public class ReviewSummaryDto
    {
        public int ProductId { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }
    }
}