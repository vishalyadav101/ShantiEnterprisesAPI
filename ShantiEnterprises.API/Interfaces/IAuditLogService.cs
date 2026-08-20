using ShantiEnterprises.API.DTOs.AuditLog;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAuditLogService
    {
        // ==========================================
        // CREATE AUDIT LOG
        // ==========================================

        Task<AuditLogResponseDto> CreateAsync(
            int? userId,
            string? userName,
            string action,
            string entityName,
            int? entityId,
            string? description,
            string? ipAddress);


        // ==========================================
        // GET ALL
        // ADMIN
        // ==========================================

        Task<List<AuditLogResponseDto>> GetAllAsync();


        // ==========================================
        // GET BY ID
        // ADMIN
        // ==========================================

        Task<AuditLogResponseDto?> GetByIdAsync(
            int auditLogId);


        // ==========================================
        // GET BY USER
        // ADMIN
        // ==========================================

        Task<List<AuditLogResponseDto>> GetByUserIdAsync(
            int userId);
    }
}