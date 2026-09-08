using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllAsync();

        Task<CouponDto?> GetByIdAsync(int id);

        Task<CouponDto?> CreateAsync(
            CouponCreateDto dto);

        Task<CouponDto?> UpdateAsync(
            int id,
            CouponUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}