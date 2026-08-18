namespace ShantiEnterprises.API.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        // =========================
        // REVIEW
        // =========================

        public int Rating { get; set; }

        public string? ReviewTitle { get; set; }

        public string? ReviewComment { get; set; }

        // =========================
        // STATUS
        // =========================

        public bool IsApproved { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // =========================
        // DATE
        // =========================

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        // =========================
        // NAVIGATION
        // =========================

        public Product? Product { get; set; }

        public User? User { get; set; }
    }
}