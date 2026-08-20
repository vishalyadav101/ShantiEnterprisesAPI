using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        // ==========================================
        // GET DASHBOARD
        // ADMIN
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var dashboard =
                    await _dashboardService
                        .GetDashboardAsync();

                return Ok(dashboard);
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