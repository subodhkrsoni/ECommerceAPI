using ECommerceAPI.DTOs;
using ECommerceAPI.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(
            ICouponService couponService)
        {
            _couponService = couponService;
        }


        // =====================================================
        // GET: api/coupons
        // Login required
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            var coupons =
                await _couponService.GetAllAsync();

            return Ok(coupons);
        }


        // =====================================================
        // GET: api/coupons/{id}
        // Login required
        // =====================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoupon(
            int id)
        {
            var coupon =
                await _couponService.GetByIdAsync(id);

            if (coupon == null)
            {
                return NotFound(new
                {
                    message = "Coupon not found"
                });
            }

            return Ok(coupon);
        }


        // =====================================================
        // POST: api/coupons
        // Admin only
        // =====================================================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCoupon(
            CouponCreateDto dto)
        {
            var coupon =
                await _couponService.CreateAsync(dto);

            if (coupon == null)
            {
                return BadRequest(new
                {
                    message =
                        "Invalid coupon data, duplicate code, invalid discount type, discount value, or expiry date."
                });
            }

            return CreatedAtAction(
                nameof(GetCoupon),
                new { id = coupon.Id },
                coupon);
        }


        // =====================================================
        // PUT: api/coupons/{id}
        // Admin only
        // =====================================================

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(
            int id,
            CouponUpdateDto dto)
        {
            var coupon =
                await _couponService.UpdateAsync(
                    id,
                    dto);

            if (coupon == null)
            {
                return BadRequest(new
                {
                    message =
                        "Coupon not found or invalid coupon data."
                });
            }

            return Ok(coupon);
        }


        // =====================================================
        // DELETE: api/coupons/{id}
        // Admin only
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(
            int id)
        {
            var deleted =
                await _couponService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Coupon not found"
                });
            }

            return NoContent();
        }
    }
}