using ShantiEnterprises.API.DTOs.AuditLog;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogService(
            IAuditLogRepository repository)
        {
            _repository = repository;
        }


        // ==========================================
        // CREATE
        // ==========================================

        public async Task<AuditLogResponseDto> CreateAsync(
            int? userId,
            string? userName,
            string action,
            string entityName,
            int? entityId,
            string? description,
            string? ipAddress)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                IpAddress = ipAddress,
                CreatedDate = DateTime.UtcNow
            };

            var created =
                await _repository.CreateAsync(auditLog);

            return MapToDto(created);
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<AuditLogResponseDto>> GetAllAsync()
        {
            var logs =
                await _repository.GetAllAsync();

            return logs
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<AuditLogResponseDto?> GetByIdAsync(
            int auditLogId)
        {
            var log =
                await _repository.GetByIdAsync(
                    auditLogId);

            if (log == null)
            {
                return null;
            }

            return MapToDto(log);
        }


        // ==========================================
        // GET BY USER
        // ==========================================

        public async Task<List<AuditLogResponseDto>> GetByUserIdAsync(
            int userId)
        {
            var logs =
                await _repository.GetByUserIdAsync(
                    userId);

            return logs
                .Select(MapToDto)
                .ToList();
        }


        // ==========================================
        // MAPPING
        // ==========================================

        private static AuditLogResponseDto MapToDto(
            AuditLog log)
        {
            return new AuditLogResponseDto
            {
                AuditLogId = log.AuditLogId,
                UserId = log.UserId,
                UserName = log.UserName,
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                Description = log.Description,
                IpAddress = log.IpAddress,
                CreatedDate = log.CreatedDate
            };
        }
    }
}