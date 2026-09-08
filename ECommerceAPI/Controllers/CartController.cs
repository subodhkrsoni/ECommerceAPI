using System.Security.Claims;

using ECommerceAPI.DTOs;
using ECommerceAPI.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(
            ICartService cartService)
        {
            _cartService = cartService;
        }


        // ==========================================
        // GET: api/Cart
        // Get current user's cart
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user token"
                });
            }

            var cart =
                await _cartService.GetCartAsync(
                    userId.Value);

            return Ok(cart);
        }


        // ==========================================
        // POST: api/Cart/items
        // Add product to cart
        // ==========================================

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(
            CartItemAddDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user token"
                });
            }

            var cart =
                await _cartService.AddItemAsync(
                    userId.Value,
                    dto);

            if (cart == null)
            {
                return BadRequest(new
                {
                    message =
                        "Product not found or insufficient stock"
                });
            }

            return Ok(cart);
        }


        // ==========================================
        // PUT: api/Cart/items/{id}
        // Update cart item quantity
        // ==========================================

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(
            int id,
            CartItemUpdateDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user token"
                });
            }

            var cart =
                await _cartService.UpdateItemAsync(
                    userId.Value,
                    id,
                    dto);

            if (cart == null)
            {
                return BadRequest(new
                {
                    message =
                        "Cart item not found or insufficient stock"
                });
            }

            return Ok(cart);
        }


        // ==========================================
        // DELETE: api/Cart/items/{id}
        // Remove item from cart
        // ==========================================

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveItem(
            int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user token"
                });
            }

            var removed =
                await _cartService.RemoveItemAsync(
                    userId.Value,
                    id);

            if (!removed)
            {
                return NotFound(new
                {
                    message = "Cart item not found"
                });
            }

            return NoContent();
        }


        // ==========================================
        // DELETE: api/Cart
        // Clear current user's cart
        // ==========================================

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user token"
                });
            }

            var cleared =
                await _cartService.ClearCartAsync(
                    userId.Value);

            if (!cleared)
            {
                return NotFound(new
                {
                    message = "Cart not found"
                });
            }

            return NoContent();
        }


        // ==========================================
        // Get User ID from JWT
        // ==========================================

        private int? GetUserId()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return null;

            if (!int.TryParse(
                    userIdClaim.Value,
                    out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}