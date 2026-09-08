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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        // ==========================================
        // Get All Payments
        // Admin Only
        // ==========================================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPayments()
        {
            var payments =
                await _paymentService.GetAllAsync();

            return Ok(payments);
        }


        // ==========================================
        // Get Payment By ID
        // ==========================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(
            int id)
        {
            var payment =
                await _paymentService.GetByIdAsync(id);

            if (payment == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(payment);
        }


        // ==========================================
        // Get Payment By Order ID
        // ==========================================

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrder(
            int orderId)
        {
            var payment =
                await _paymentService
                    .GetByOrderIdAsync(orderId);

            if (payment == null)
            {
                return NotFound(new
                {
                    message =
                        "Payment not found for this order."
                });
            }

            return Ok(payment);
        }


        // ==========================================
        // Create Payment
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            PaymentCreateDto dto)
        {
            try
            {
                // ==========================================
                // Get Logged-in User ID
                // ==========================================

                var userIdClaim =
                    User.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(
                        userIdClaim,
                        out int userId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Invalid user token."
                    });
                }


                // ==========================================
                // Create Payment
                // ==========================================

                var payment =
                    await _paymentService
                        .CreatePaymentAsync(
                            userId,
                            dto);


                // ==========================================
                // Validation Failed
                // ==========================================

                if (payment == null)
                {
                    return BadRequest(new
                    {
                        message =
                            "Payment failed. Please check the order, payment method, or existing payment."
                    });
                }


                // ==========================================
                // Success
                // ==========================================

                return StatusCode(
                    StatusCodes.Status201Created,
                    payment);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}