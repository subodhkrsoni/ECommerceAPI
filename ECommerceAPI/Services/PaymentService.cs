using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;

namespace ECommerceAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ApplicationDbContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }


        // ==========================================
        // Get All Payments
        // ==========================================

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments =
                await _paymentRepository.GetAllAsync();

            return payments.Select(MapToDto);
        }


        // ==========================================
        // Get Payment By ID
        // ==========================================

        public async Task<PaymentDto?> GetByIdAsync(
            int id)
        {
            var payment =
                await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return null;
            }

            return MapToDto(payment);
        }


        // ==========================================
        // Get Payment By Order ID
        // ==========================================

        public async Task<PaymentDto?> GetByOrderIdAsync(
            int orderId)
        {
            var payment =
                await _paymentRepository
                    .GetByOrderIdAsync(orderId);

            if (payment == null)
            {
                return null;
            }

            return MapToDto(payment);
        }


        // ==========================================
        // Create Payment
        // ==========================================

        public async Task<PaymentDto?> CreatePaymentAsync(
            int userId,
            PaymentCreateDto dto)
        {
            // ==========================================
            // Check Order
            // ==========================================

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == dto.OrderId);

            if (order == null)
            {
                return null;
            }


            // ==========================================
            // Check Customer Ownership
            // ==========================================

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Id == order.CustomerId);

            if (customer == null)
            {
                return null;
            }


            // ==========================================
            // Check Existing Payment
            // ==========================================

            var existingPayment =
                await _paymentRepository
                    .GetByOrderIdAsync(dto.OrderId);

            if (existingPayment != null)
            {
                return null;
            }


            // ==========================================
            // Validate Payment Method
            // ==========================================

            var paymentMethod =
                dto.PaymentMethod.Trim();

            if (!paymentMethod.Equals(
                    "UPI",
                    StringComparison.OrdinalIgnoreCase) &&
                !paymentMethod.Equals(
                    "Card",
                    StringComparison.OrdinalIgnoreCase) &&
                !paymentMethod.Equals(
                    "Cash",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }


            // ==========================================
            // Create Transaction ID
            // ==========================================

            string? transactionId = null;

            if (!dto.SimulateFailure)
            {
                transactionId =
                    "TXN-" +
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 12)
                        .ToUpper();
            }


            // ==========================================
            // Simulate Payment
            // ==========================================

            var paymentStatus =
    dto.SimulateFailure
        ? "Failed"
        : "Success";


            // ==========================================
            // Create Payment
            // ==========================================

            var payment = new Payment
            {
                OrderId = order.Id,

                PaymentMethod =
                    paymentMethod.ToUpper(),

                PaymentStatus =
                    paymentStatus,

                Amount =
                    order.TotalAmount,

                TransactionId =
                    transactionId,

                CreatedAt =
                    DateTime.UtcNow
            };


            // ==========================================
            // Save Payment
            // ==========================================

            var createdPayment =
                await _paymentRepository
                    .AddAsync(payment);


            // ==========================================
            // Update Order Status
            // ==========================================

            if (paymentStatus == "Success")
            {
                order.Status = "Confirmed";

                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();


            return MapToDto(createdPayment);
        }


        // ==========================================
        // Map Entity → DTO
        // ==========================================

        private static PaymentDto MapToDto(
            Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,

                OrderId =
                    payment.OrderId,

                PaymentMethod =
                    payment.PaymentMethod,

                PaymentStatus =
                    payment.PaymentStatus,

                Amount =
                    payment.Amount,

                TransactionId =
                    payment.TransactionId,

                CreatedAt =
                    payment.CreatedAt
            };
        }
    }
}