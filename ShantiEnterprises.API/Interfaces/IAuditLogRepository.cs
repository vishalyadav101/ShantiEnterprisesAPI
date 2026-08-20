using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IAuditLogRepository
    {
        // ==========================================
        // CREATE
        // ==========================================

        Task<AuditLog> CreateAsync(
            AuditLog auditLog);


        // ==========================================
        // GET ALL
        // ==========================================

        Task<List<AuditLog>> GetAllAsync();


        // ==========================================
        // GET BY ID
        // ==========================================

        Task<AuditLog?> GetByIdAsync(
            int auditLogId);


        // ==========================================
        // GET BY USER
        // ==========================================

        Task<List<AuditLog>> GetByUserIdAsync(
            int userId);
    }
}