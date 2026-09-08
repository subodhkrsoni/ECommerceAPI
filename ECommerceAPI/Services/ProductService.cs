using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<ProductDto> Data, int TotalRecords)>
            GetAllAsync(ProductQueryParameters parameters)
        {
            if (parameters.PageNumber < 1)
                parameters.PageNumber = 1;

            if (parameters.PageSize < 1)
                parameters.PageSize = 10;

            var products = await _productRepository.GetAllAsync(
                parameters.Search,
                parameters.CategoryId,
                parameters.SortBy,
                parameters.SortOrder,
                parameters.PageNumber,
                parameters.PageSize);

            var totalRecords =
                await _productRepository.GetTotalCountAsync(
                    parameters.Search,
                    parameters.CategoryId);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty
            });

            return (productDtos, totalRecords);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task<ProductDto?> CreateAsync(
            ProductCreateDto dto)
        {
            var categoryExists =
                await _productRepository
                    .CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
                return null;

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            var createdProduct =
                await _productRepository.AddAsync(product);

            return await GetByIdAsync(createdProduct.Id);
        }

        public async Task<ProductDto?> UpdateAsync(
            int id,
            ProductUpdateDto dto)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return null;

            var categoryExists =
                await _productRepository
                    .CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
                return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _productRepository.UpdateAsync(product);

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }
    }
}