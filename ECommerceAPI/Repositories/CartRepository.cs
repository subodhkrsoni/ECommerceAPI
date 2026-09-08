using ECommerceAPI.Data;
using ECommerceAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // Get Cart By User ID
        // ==========================================

        public async Task<Cart?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c => c.UserId == userId);
        }


        // ==========================================
        // Create Cart
        // ==========================================

        public async Task<Cart> CreateAsync(
            Cart cart)
        {
            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }


        // ==========================================
        // Get Product By ID
        // ==========================================

        public async Task<Product?> GetProductAsync(
            int productId)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == productId);
        }


        // ==========================================
        // Get Cart Item By Product
        // ==========================================

        public async Task<CartItem?> GetCartItemAsync(
            int cartId,
            int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cartId &&
                    ci.ProductId == productId);
        }


        // ==========================================
        // Get Cart Item By ID
        // ==========================================

        public async Task<CartItem?> GetCartItemByIdAsync(
            int cartId,
            int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci =>
                    ci.Id == cartItemId &&
                    ci.CartId == cartId);
        }


        // ==========================================
        // Add Cart Item
        // ==========================================

        public async Task<CartItem> AddItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();

            return cartItem;
        }


        // ==========================================
        // Update Cart Item
        // ==========================================

        public async Task UpdateItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // Remove Cart Item
        // ==========================================

        public async Task RemoveItemAsync(
            CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // Clear Cart Items
        // ==========================================

        public async Task ClearItemsAsync(
            Cart cart)
        {
            _context.CartItems.RemoveRange(
                cart.CartItems);

            await _context.SaveChangesAsync();
        }


        // ==========================================
        // Save Changes
        // ==========================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}