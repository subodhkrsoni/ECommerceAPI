using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var totalProducts =
                await _context.Products.CountAsync();

            var totalCustomers =
                await _context.Customers.CountAsync();

            var totalOrders =
                await _context.Orders.CountAsync();

            var totalRevenue =
                await _context.Orders
                    .Where(o =>
                        o.Status != "Cancelled")
                    .SumAsync(o => o.TotalAmount);

            var pendingOrders =
                await _context.Orders
                    .CountAsync(o =>
                        o.Status == "Pending");

            var confirmedOrders =
                await _context.Orders
                    .CountAsync(o =>
                        o.Status == "Confirmed");

            var shippedOrders =
                await _context.Orders
                    .CountAsync(o =>
                        o.Status == "Shipped");

            var deliveredOrders =
                await _context.Orders
                    .CountAsync(o =>
                        o.Status == "Delivered");

            var cancelledOrders =
                await _context.Orders
                    .CountAsync(o =>
                        o.Status == "Cancelled");

            var successfulPayments =
                await _context.Payments
                    .CountAsync(p =>
                        p.PaymentStatus == "Success");

            var failedPayments =
                await _context.Payments
                    .CountAsync(p =>
                        p.PaymentStatus == "Failed");

            return new AdminDashboardDto
            {
                TotalProducts = totalProducts,

                TotalCustomers = totalCustomers,

                TotalOrders = totalOrders,

                TotalRevenue = totalRevenue,

                PendingOrders = pendingOrders,

                ConfirmedOrders = confirmedOrders,

                ShippedOrders = shippedOrders,

                DeliveredOrders = deliveredOrders,

                CancelledOrders = cancelledOrders,

                SuccessfulPayments =
                    successfulPayments,

                FailedPayments =
                    failedPayments
            };
        }
    }
}