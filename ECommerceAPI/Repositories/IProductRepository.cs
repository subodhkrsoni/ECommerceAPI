using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(
            string? search = null,
            int? categoryId = null,
            string? sortBy = null,
            string? sortOrder = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<int> GetTotalCountAsync(
            string? search = null,
            int? categoryId = null);

        Task<Product?> GetByIdAsync(int id);

        Task<Product> AddAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(Product product);

        Task<bool> ExistsAsync(int id);

        Task<bool> CategoryExistsAsync(int categoryId);
    }
}