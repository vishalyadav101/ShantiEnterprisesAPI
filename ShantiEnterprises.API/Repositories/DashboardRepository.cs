using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public DashboardRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // USERS
        // ==========================================

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users
                .CountAsync();
        }


        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Users
                .CountAsync(x => x.Role == "Customer");
        }


        public async Task<int> GetTotalAdminsAsync()
        {
            return await _context.Users
                .CountAsync(x => x.Role == "Admin");
        }


        // ==========================================
        // PRODUCTS
        // ==========================================

        public async Task<int> GetTotalProductsAsync()
        {
            return await _context.Products
                .CountAsync();
        }


        public async Task<int> GetActiveProductsAsync()
        {
            return await _context.Products
                .CountAsync(x => x.IsActive);
        }


        public async Task<int> GetInactiveProductsAsync()
        {
            return await _context.Products
                .CountAsync(x => !x.IsActive);
        }


        // ==========================================
        // ORDERS
        // ==========================================

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders
                .CountAsync();
        }


        public async Task<int> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .CountAsync(x => x.OrderStatus == "Pending");
        }


        public async Task<int> GetConfirmedOrdersAsync()
        {
            return await _context.Orders
                .CountAsync(x => x.OrderStatus == "Confirmed");
        }


        public async Task<int> GetDeliveredOrdersAsync()
        {
            return await _context.Orders
                .CountAsync(x => x.OrderStatus == "Delivered");
        }


        public async Task<int> GetCancelledOrdersAsync()
        {
            return await _context.Orders
                .CountAsync(x => x.OrderStatus == "Cancelled");
        }


        // ==========================================
        // PAYMENTS
        // ==========================================

        public async Task<int> GetTotalPaymentsAsync()
        {
            return await _context.Payments
                .CountAsync();
        }


        public async Task<int> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .CountAsync(x => x.PaymentStatus == "Pending");
        }


        public async Task<int> GetPaidPaymentsAsync()
        {
            return await _context.Payments
                .CountAsync(x => x.PaymentStatus == "Paid");
        }


        public async Task<int> GetFailedPaymentsAsync()
        {
            return await _context.Payments
                .CountAsync(x => x.PaymentStatus == "Failed");
        }


        // ==========================================
        // RETURNS
        // ==========================================

        public async Task<int> GetTotalReturnsAsync()
        {
            return await _context.Returns
                .CountAsync();
        }


        public async Task<int> GetPendingReturnsAsync()
        {
            return await _context.Returns
                .CountAsync(x => x.ReturnStatus == "Pending");
        }


        public async Task<int> GetApprovedReturnsAsync()
        {
            return await _context.Returns
                .CountAsync(x => x.ReturnStatus == "Approved");
        }


        public async Task<int> GetCompletedReturnsAsync()
        {
            return await _context.Returns
                .CountAsync(x => x.ReturnStatus == "Completed");
        }


        // ==========================================
        // REFUNDS
        // ==========================================

        public async Task<int> GetTotalRefundsAsync()
        {
            return await _context.Refunds
                .CountAsync();
        }


        public async Task<int> GetPendingRefundsAsync()
        {
            return await _context.Refunds
                .CountAsync(x => x.RefundStatus == "Pending");
        }


        public async Task<int> GetCompletedRefundsAsync()
        {
            return await _context.Refunds
                .CountAsync(x => x.RefundStatus == "Completed");
        }


        // ==========================================
        // REVENUE
        // ==========================================

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(x => x.PaymentStatus == "Paid")
                .SumAsync(x => (decimal?)x.GrandTotal) ?? 0;
        }


        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.Today;

            return await _context.Orders
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.CreatedDate >= today)
                .SumAsync(x => (decimal?)x.GrandTotal) ?? 0;
        }


        public async Task<decimal> GetMonthlyRevenueAsync()
        {
            var today = DateTime.Today;

            var startOfMonth =
                new DateTime(
                    today.Year,
                    today.Month,
                    1);

            return await _context.Orders
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.CreatedDate >= startOfMonth)
                .SumAsync(x => (decimal?)x.GrandTotal) ?? 0;
        }
    }
}