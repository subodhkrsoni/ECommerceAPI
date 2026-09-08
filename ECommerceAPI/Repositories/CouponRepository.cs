using ECommerceAPI.Data;
using ECommerceAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // Get All Coupons
        // ==========================================

        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _context.Coupons
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }


        // ==========================================
        // Get Coupon By ID
        // ==========================================

        public async Task<Coupon?> GetByIdAsync(int id)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c =>
                    c.Id == id);
        }


        // ==========================================
        // Get Coupon By Code
        // ==========================================

        public async Task<Coupon?> GetByCodeAsync(
            string code)
        {
            return await _context.Coupons
                .FirstOrDefaultAsync(c =>
                    c.Code == code);
        }


        // ==========================================
        // Check Duplicate Coupon Code
        // ==========================================

        public async Task<bool> ExistsByCodeAsync(
            string code,
            int? excludeId = null)
        {
            var query = _context.Coupons
                .AsQueryable();

            query = query.Where(c =>
                c.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(c =>
                    c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }


        // ==========================================
        // Add Coupon
        // ==========================================

        public async Task<Coupon> AddAsync(
            Coupon coupon)
        {
            _context.Coupons.Add(coupon);

            await _context.SaveChangesAsync();

            return coupon;
        }


        // ==========================================
        // Update Coupon
        // ==========================================

        public async Task UpdateAsync(
            Coupon coupon)
        {
            _context.Coupons.Update(coupon);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // Delete Coupon
        // ==========================================

        public async Task DeleteAsync(
            Coupon coupon)
        {
            _context.Coupons.Remove(coupon);

            await _context.SaveChangesAsync();
        }
    }
}