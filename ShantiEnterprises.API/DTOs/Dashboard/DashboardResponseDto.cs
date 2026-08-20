namespace ShantiEnterprises.API.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        // ==========================================
        // USERS
        // ==========================================

        public int TotalUsers { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalAdmins { get; set; }


        // ==========================================
        // PRODUCTS
        // ==========================================

        public int TotalProducts { get; set; }

        public int ActiveProducts { get; set; }

        public int InactiveProducts { get; set; }


        // ==========================================
        // ORDERS
        // ==========================================

        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }

        public int ConfirmedOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int CancelledOrders { get; set; }


        // ==========================================
        // PAYMENTS
        // ==========================================

        public int TotalPayments { get; set; }

        public int PendingPayments { get; set; }

        public int PaidPayments { get; set; }

        public int FailedPayments { get; set; }


        // ==========================================
        // RETURNS
        // ==========================================

        public int TotalReturns { get; set; }

        public int PendingReturns { get; set; }

        public int ApprovedReturns { get; set; }

        public int CompletedReturns { get; set; }


        // ==========================================
        // REFUNDS
        // ==========================================

        public int TotalRefunds { get; set; }

        public int PendingRefunds { get; set; }

        public int CompletedRefunds { get; set; }


        // ==========================================
        // REVENUE
        // ==========================================

        public decimal TotalRevenue { get; set; }

        public decimal TodayRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }
    }
}