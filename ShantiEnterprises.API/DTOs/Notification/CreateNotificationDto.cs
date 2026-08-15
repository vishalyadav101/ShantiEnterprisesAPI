namespace ShantiEnterprises.API.DTOs.Notification
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Type { get; set; }

        public string? ReferenceType { get; set; }

        public int? ReferenceId { get; set; }
    }
}