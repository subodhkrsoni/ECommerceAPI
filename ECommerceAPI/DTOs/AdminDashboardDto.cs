namespace ECommerceAPI.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }

        public int ConfirmedOrders { get; set; }

        public int ShippedOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int CancelledOrders { get; set; }

        public int SuccessfulPayments { get; set; }

        public int FailedPayments { get; set; }
    }
}