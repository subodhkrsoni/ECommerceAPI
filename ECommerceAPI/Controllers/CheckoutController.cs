using ECommerceAPI.DTOs;
using ECommerceAPI.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(
            ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }


        // ==========================================
        // Checkout
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> Checkout(
            CheckoutCreateDto dto)
        {
            try
            {
                // ==========================================
                // Get Logged-in User ID
                // ==========================================

                var userIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new
                    {
                        message = "Invalid user token."
                    });
                }


                // ==========================================
                // Process Checkout
                // ==========================================

                var order =
                    await _checkoutService.CheckoutAsync(
                        userId,
                        dto);


                // ==========================================
                // Checkout Validation Failed
                // ==========================================

                if (order == null)
                {
                    return BadRequest(new
                    {
                        message =
                            "Checkout failed. Please check your cart, customer, products, and stock."
                    });
                }


                // ==========================================
                // Success
                // ==========================================

                return StatusCode(
                    StatusCodes.Status201Created,
                    order);
            }
            catch (Exception ex)
            {
                // ==========================================
                // Business Validation Error
                // ==========================================

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}