using System.ComponentModel.DataAnnotations;

namespace ShantiEnterprises.API.DTOs.AuditLog
{
    public class CreateAuditLogDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string? Description { get; set; }
    }
}