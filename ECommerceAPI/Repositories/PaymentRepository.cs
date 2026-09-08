using ECommerceAPI.Data;
using ECommerceAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // Get All Payments
        // ==========================================

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }


        // ==========================================
        // Get Payment By ID
        // ==========================================

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.Id == id);
        }


        // ==========================================
        // Get Payment By Order ID
        // ==========================================

        public async Task<Payment?> GetByOrderIdAsync(
            int orderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.OrderId == orderId);
        }


        // ==========================================
        // Add Payment
        // ==========================================

        public async Task<Payment> AddAsync(
            Payment payment)
        {
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return payment;
        }


        // ==========================================
        // Update Payment
        // ==========================================

        public async Task UpdateAsync(
            Payment payment)
        {
            _context.Payments.Update(payment);

            await _context.SaveChangesAsync();
        }
    }
}