using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}