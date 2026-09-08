using ECommerceAPI.DTOs;

namespace ECommerceAPI.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto> Data, int TotalRecords)>
            GetAllAsync(ProductQueryParameters parameters);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto?> CreateAsync(ProductCreateDto dto);

        Task<ProductDto?> UpdateAsync(
            int id,
            ProductUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}