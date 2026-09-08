using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(int id);

        Task<Payment?> GetByOrderIdAsync(int orderId);

        Task<Payment> AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);
    }
}