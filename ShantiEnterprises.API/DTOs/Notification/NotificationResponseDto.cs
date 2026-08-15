namespace ShantiEnterprises.API.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Type { get; set; }

        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}