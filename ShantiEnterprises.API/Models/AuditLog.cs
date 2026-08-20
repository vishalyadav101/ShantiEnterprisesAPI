namespace ShantiEnterprises.API.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public string Action { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string? Description { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}