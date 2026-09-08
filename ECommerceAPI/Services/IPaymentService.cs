using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();

        Task<PaymentDto?> GetByIdAsync(int id);

        Task<PaymentDto?> GetByOrderIdAsync(int orderId);

        Task<PaymentDto?> CreatePaymentAsync(
            int userId,
            PaymentCreateDto dto);
    }
}