using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface ICartRepository
    {
        // Get user's cart with cart items and products
        Task<Cart?> GetByUserIdAsync(int userId);

        // Create a new cart
        Task<Cart> CreateAsync(Cart cart);

        // Get product by ID
        Task<Product?> GetProductAsync(int productId);

        // Get cart item by Cart ID + Product ID
        Task<CartItem?> GetCartItemAsync(
            int cartId,
            int productId);

        // Get cart item by Cart ID + Cart Item ID
        Task<CartItem?> GetCartItemByIdAsync(
            int cartId,
            int cartItemId);

        // Add item to cart
        Task<CartItem> AddItemAsync(
            CartItem cartItem);

        // Update cart item
        Task UpdateItemAsync(
            CartItem cartItem);

        // Remove cart item
        Task RemoveItemAsync(
            CartItem cartItem);

        // Remove all items from cart
        Task ClearItemsAsync(
            Cart cart);

        // Save database changes
        Task SaveChangesAsync();
    }
}