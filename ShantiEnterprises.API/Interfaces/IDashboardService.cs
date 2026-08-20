using ShantiEnterprises.API.DTOs.Dashboard;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IDashboardService
    {
        // ==========================================
        // GET DASHBOARD SUMMARY
        // ADMIN
        // ==========================================

        Task<DashboardResponseDto> GetDashboardAsync();
    }
}