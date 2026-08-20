using ShantiEnterprises.API.DTOs.Dashboard;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }


        // ==========================================
        // GET DASHBOARD
        // ==========================================

        public async Task<DashboardResponseDto> GetDashboardAsync()
        {
            var dashboard = new DashboardResponseDto
            {
                // ==================================
                // USERS
                // ==================================

                TotalUsers =
                    await _dashboardRepository
                        .GetTotalUsersAsync(),

                TotalCustomers =
                    await _dashboardRepository
                        .GetTotalCustomersAsync(),

                TotalAdmins =
                    await _dashboardRepository
                        .GetTotalAdminsAsync(),


                // ==================================
                // PRODUCTS
                // ==================================

                TotalProducts =
                    await _dashboardRepository
                        .GetTotalProductsAsync(),

                ActiveProducts =
                    await _dashboardRepository
                        .GetActiveProductsAsync(),

                InactiveProducts =
                    await _dashboardRepository
                        .GetInactiveProductsAsync(),


                // ==================================
                // ORDERS
                // ==================================

                TotalOrders =
                    await _dashboardRepository
                        .GetTotalOrdersAsync(),

                PendingOrders =
                    await _dashboardRepository
                        .GetPendingOrdersAsync(),

                ConfirmedOrders =
                    await _dashboardRepository
                        .GetConfirmedOrdersAsync(),

                DeliveredOrders =
                    await _dashboardRepository
                        .GetDeliveredOrdersAsync(),

                CancelledOrders =
                    await _dashboardRepository
                        .GetCancelledOrdersAsync(),


                // ==================================
                // PAYMENTS
                // ==================================

                TotalPayments =
                    await _dashboardRepository
                        .GetTotalPaymentsAsync(),

                PendingPayments =
                    await _dashboardRepository
                        .GetPendingPaymentsAsync(),

                PaidPayments =
                    await _dashboardRepository
                        .GetPaidPaymentsAsync(),

                FailedPayments =
                    await _dashboardRepository
                        .GetFailedPaymentsAsync(),


                // ==================================
                // RETURNS
                // ==================================

                TotalReturns =
                    await _dashboardRepository
                        .GetTotalReturnsAsync(),

                PendingReturns =
                    await _dashboardRepository
                        .GetPendingReturnsAsync(),

                ApprovedReturns =
                    await _dashboardRepository
                        .GetApprovedReturnsAsync(),

                CompletedReturns =
                    await _dashboardRepository
                        .GetCompletedReturnsAsync(),


                // ==================================
                // REFUNDS
                // ==================================

                TotalRefunds =
                    await _dashboardRepository
                        .GetTotalRefundsAsync(),

                PendingRefunds =
                    await _dashboardRepository
                        .GetPendingRefundsAsync(),

                CompletedRefunds =
                    await _dashboardRepository
                        .GetCompletedRefundsAsync(),


                // ==================================
                // REVENUE
                // ==================================

                TotalRevenue =
                    await _dashboardRepository
                        .GetTotalRevenueAsync(),

                TodayRevenue =
                    await _dashboardRepository
                        .GetTodayRevenueAsync(),

                MonthlyRevenue =
                    await _dashboardRepository
                        .GetMonthlyRevenueAsync()
            };

            return dashboard;
        }
    }
}