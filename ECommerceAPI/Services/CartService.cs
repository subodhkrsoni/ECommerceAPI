using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(
            ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }


        // ==========================================
        // Get Cart
        // ==========================================

        public async Task<CartDto> GetCartAsync(
            int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cartRepository.CreateAsync(cart);

                cart =
                    await _cartRepository.GetByUserIdAsync(userId);
            }

            return MapToDto(cart!);
        }


        // ==========================================
        // Add Item To Cart
        // ==========================================

        public async Task<CartDto?> AddItemAsync(
            int userId,
            CartItemAddDto dto)
        {
            // Check product
            var product =
                await _cartRepository.GetProductAsync(
                    dto.ProductId);

            if (product == null)
                return null;

            // Check stock
            if (dto.Quantity > product.Stock)
                return null;


            // Get user's cart
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);


            // Create cart if it doesn't exist
            if (cart == null)
            {
                var newCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cartRepository.CreateAsync(newCart);

                cart =
                    await _cartRepository.GetByUserIdAsync(userId);

                if (cart == null)
                    return null;
            }


            // Check if product already exists
            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    cart.Id,
                    dto.ProductId);


            // ==========================================
            // Product already in cart
            // ==========================================

            if (existingItem != null)
            {
                var newQuantity =
                    existingItem.Quantity + dto.Quantity;

                if (newQuantity > product.Stock)
                    return null;

                existingItem.Quantity = newQuantity;

                await _cartRepository.UpdateItemAsync(
                    existingItem);
            }


            // ==========================================
            // New product
            // ==========================================

            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                await _cartRepository.AddItemAsync(
                    cartItem);
            }


            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.SaveChangesAsync();


            // Return updated cart
            return await GetCartAsync(userId);
        }


        // ==========================================
        // Update Cart Item
        // ==========================================

        public async Task<CartDto?> UpdateItemAsync(
            int userId,
            int cartItemId,
            CartItemUpdateDto dto)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                return null;


            var cartItem =
                await _cartRepository.GetCartItemByIdAsync(
                    cart.Id,
                    cartItemId);

            if (cartItem == null)
                return null;


            if (cartItem.Product == null)
                return null;


            if (dto.Quantity > cartItem.Product.Stock)
                return null;


            cartItem.Quantity = dto.Quantity;

            cart.UpdatedAt = DateTime.UtcNow;


            await _cartRepository.UpdateItemAsync(
                cartItem);

            return await GetCartAsync(userId);
        }


        // ==========================================
        // Remove Item
        // ==========================================

        public async Task<bool> RemoveItemAsync(
            int userId,
            int cartItemId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                return false;


            var cartItem =
                await _cartRepository.GetCartItemByIdAsync(
                    cart.Id,
                    cartItemId);

            if (cartItem == null)
                return false;


            await _cartRepository.RemoveItemAsync(
                cartItem);

            return true;
        }


        // ==========================================
        // Clear Cart
        // ==========================================

        public async Task<bool> ClearCartAsync(
            int userId)
        {
            var cart =
                await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
                return false;


            await _cartRepository.ClearItemsAsync(
                cart);

            return true;
        }


        // ==========================================
        // Map Entity -> DTO
        // ==========================================

        private static CartDto MapToDto(
            Cart cart)
        {
            var items = cart.CartItems
                .Select(item => new CartItemDto
                {
                    Id = item.Id,

                    ProductId = item.ProductId,

                    ProductName =
                        item.Product?.Name
                        ?? string.Empty,

                    ImageUrl =
                        item.Product?.ImageUrl,

                    UnitPrice =
                        item.Product?.Price ?? 0,

                    Quantity =
                        item.Quantity,

                    TotalPrice =
                        (item.Product?.Price ?? 0)
                        * item.Quantity,

                    AvailableStock =
                        item.Product?.Stock ?? 0
                })
                .ToList();


            return new CartDto
            {
                Id = cart.Id,

                UserId = cart.UserId,

                TotalAmount =
                    items.Sum(i => i.TotalPrice),

                Items = items
            };
        }
    }
}