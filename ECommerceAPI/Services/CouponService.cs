using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(
            ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }


        // ==========================================
        // Get All
        // ==========================================

        public async Task<IEnumerable<CouponDto>> GetAllAsync()
        {
            var coupons =
                await _couponRepository.GetAllAsync();

            return coupons.Select(MapToDto);
        }


        // ==========================================
        // Get By ID
        // ==========================================

        public async Task<CouponDto?> GetByIdAsync(
            int id)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(id);

            if (coupon == null)
                return null;

            return MapToDto(coupon);
        }


        // ==========================================
        // Create
        // ==========================================

        public async Task<CouponDto?> CreateAsync(
            CouponCreateDto dto)
        {
            var code = dto.Code
                .Trim()
                .ToUpperInvariant();

            // Validate discount type
            if (dto.DiscountType != "Percentage" &&
                dto.DiscountType != "Fixed")
            {
                return null;
            }


            // Percentage cannot exceed 100
            if (dto.DiscountType == "Percentage" &&
                dto.DiscountValue > 100)
            {
                return null;
            }


            // Expiry must be future
            if (dto.ExpiryDate <= DateTime.UtcNow)
            {
                return null;
            }


            // Check duplicate code
            var exists =
                await _couponRepository
                    .ExistsByCodeAsync(code);

            if (exists)
                return null;


            var coupon = new Coupon
            {
                Code = code,

                DiscountType =
                    dto.DiscountType,

                DiscountValue =
                    dto.DiscountValue,

                MinimumOrderAmount =
                    dto.MinimumOrderAmount,

                ExpiryDate =
                    dto.ExpiryDate,

                IsActive =
                    dto.IsActive,

                CreatedAt =
                    DateTime.UtcNow
            };


            var created =
                await _couponRepository
                    .AddAsync(coupon);

            return MapToDto(created);
        }


        // ==========================================
        // Update
        // ==========================================

        public async Task<CouponDto?> UpdateAsync(
            int id,
            CouponUpdateDto dto)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(id);

            if (coupon == null)
                return null;


            var code = dto.Code
                .Trim()
                .ToUpperInvariant();


            // Validate discount type
            if (dto.DiscountType != "Percentage" &&
                dto.DiscountType != "Fixed")
            {
                return null;
            }


            // Percentage cannot exceed 100
            if (dto.DiscountType == "Percentage" &&
                dto.DiscountValue > 100)
            {
                return null;
            }


            // Expiry must be future
            if (dto.ExpiryDate <= DateTime.UtcNow)
            {
                return null;
            }


            // Check duplicate code
            var exists =
                await _couponRepository
                    .ExistsByCodeAsync(
                        code,
                        id);

            if (exists)
                return null;


            coupon.Code = code;

            coupon.DiscountType =
                dto.DiscountType;

            coupon.DiscountValue =
                dto.DiscountValue;

            coupon.MinimumOrderAmount =
                dto.MinimumOrderAmount;

            coupon.ExpiryDate =
                dto.ExpiryDate;

            coupon.IsActive =
                dto.IsActive;


            await _couponRepository
                .UpdateAsync(coupon);

            return MapToDto(coupon);
        }


        // ==========================================
        // Delete
        // ==========================================

        public async Task<bool> DeleteAsync(
            int id)
        {
            var coupon =
                await _couponRepository.GetByIdAsync(id);

            if (coupon == null)
                return false;

            await _couponRepository.DeleteAsync(coupon);

            return true;
        }


        // ==========================================
        // Mapping
        // ==========================================

        private static CouponDto MapToDto(
            Coupon coupon)
        {
            return new CouponDto
            {
                Id = coupon.Id,

                Code = coupon.Code,

                DiscountType =
                    coupon.DiscountType,

                DiscountValue =
                    coupon.DiscountValue,

                MinimumOrderAmount =
                    coupon.MinimumOrderAmount,

                ExpiryDate =
                    coupon.ExpiryDate,

                IsActive =
                    coupon.IsActive,

                CreatedAt =
                    coupon.CreatedAt
            };
        }
    }
}