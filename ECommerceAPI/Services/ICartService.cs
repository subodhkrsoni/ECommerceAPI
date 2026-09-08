using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);

        Task<CartDto?> AddItemAsync(
            int userId,
            CartItemAddDto dto);

        Task<CartDto?> UpdateItemAsync(
            int userId,
            int cartItemId,
            CartItemUpdateDto dto);

        Task<bool> RemoveItemAsync(
            int userId,
            int cartItemId);

        Task<bool> ClearCartAsync(
            int userId);
    }
}