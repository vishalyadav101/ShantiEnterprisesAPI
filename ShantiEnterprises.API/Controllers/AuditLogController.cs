using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }


        // ==========================================
        // GET ALL AUDIT LOGS
        // ADMIN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var logs =
                    await _auditLogService.GetAllAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // GET AUDIT LOG BY ID
        // ADMIN
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            try
            {
                var log =
                    await _auditLogService
                        .GetByIdAsync(id);

                if (log == null)
                {
                    return NotFound(new
                    {
                        message = "Audit log not found."
                    });
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // ==========================================
        // GET AUDIT LOGS BY USER
        // ADMIN
        // ==========================================

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(
            int userId)
        {
            try
            {
                var logs =
                    await _auditLogService
                        .GetByUserIdAsync(userId);

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}