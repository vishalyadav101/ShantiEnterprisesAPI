using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public AuditLogRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // CREATE
        // ==========================================

        public async Task<AuditLog> CreateAsync(
            AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            return auditLog;
        }


        // ==========================================
        // GET ALL
        // ==========================================

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }


        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<AuditLog?> GetByIdAsync(
            int auditLogId)
        {
            return await _context.AuditLogs
                .FirstOrDefaultAsync(x =>
                    x.AuditLogId == auditLogId);
        }


        // ==========================================
        // GET BY USER
        // ==========================================

        public async Task<List<AuditLog>> GetByUserIdAsync(
            int userId)
        {
            return await _context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}