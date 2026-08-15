namespace ShantiEnterprises.API.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }
            = string.Empty;

        public string Message { get; set; }
            = string.Empty;

        public string? Type { get; set; }

        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        public DateTime? ReadDate { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}