namespace ShantiEnterprises.API.Interfaces
{
    public interface IDashboardRepository
    {
        // ==========================================
        // USERS
        // ==========================================

        Task<int> GetTotalUsersAsync();

        Task<int> GetTotalCustomersAsync();

        Task<int> GetTotalAdminsAsync();


        // ==========================================
        // PRODUCTS
        // ==========================================

        Task<int> GetTotalProductsAsync();

        Task<int> GetActiveProductsAsync();

        Task<int> GetInactiveProductsAsync();


        // ==========================================
        // ORDERS
        // ==========================================

        Task<int> GetTotalOrdersAsync();

        Task<int> GetPendingOrdersAsync();

        Task<int> GetConfirmedOrdersAsync();

        Task<int> GetDeliveredOrdersAsync();

        Task<int> GetCancelledOrdersAsync();


        // ==========================================
        // PAYMENTS
        // ==========================================

        Task<int> GetTotalPaymentsAsync();

        Task<int> GetPendingPaymentsAsync();

        Task<int> GetPaidPaymentsAsync();

        Task<int> GetFailedPaymentsAsync();


        // ==========================================
        // RETURNS
        // ==========================================

        Task<int> GetTotalReturnsAsync();

        Task<int> GetPendingReturnsAsync();

        Task<int> GetApprovedReturnsAsync();

        Task<int> GetCompletedReturnsAsync();


        // ==========================================
        // REFUNDS
        // ==========================================

        Task<int> GetTotalRefundsAsync();

        Task<int> GetPendingRefundsAsync();

        Task<int> GetCompletedRefundsAsync();


        // ==========================================
        // REVENUE
        // ==========================================

        Task<decimal> GetTotalRevenueAsync();

        Task<decimal> GetTodayRevenueAsync();

        Task<decimal> GetMonthlyRevenueAsync();
    }
}