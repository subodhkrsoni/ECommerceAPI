using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface ICheckoutService
    {
        Task<OrderDto?> CheckoutAsync(
            int userId,
            CheckoutCreateDto dto);
    }
}