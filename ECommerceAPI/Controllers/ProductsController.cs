using ECommerceAPI.DTOs;
using ECommerceAPI.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProducts(
            [FromQuery] ProductQueryParameters parameters)
        {
            var result =
                await _productService.GetAllAsync(parameters);

            var totalPages = (int)Math.Ceiling(
                result.TotalRecords /
                (double)parameters.PageSize);

            return Ok(new
            {
                pageNumber = parameters.PageNumber,
                pageSize = parameters.PageSize,
                totalRecords = result.TotalRecords,
                totalPages = totalPages,
                data = result.Data
            });
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product =
                await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(
            ProductCreateDto dto)
        {
            var product =
                await _productService.CreateAsync(dto);

            if (product == null)
            {
                return BadRequest(new
                {
                    message = "Category not found"
                });
            }

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            ProductUpdateDto dto)
        {
            var existingProduct =
                await _productService.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            var updatedProduct =
                await _productService.UpdateAsync(id, dto);

            if (updatedProduct == null)
            {
                return BadRequest(new
                {
                    message = "Category not found"
                });
            }

            return Ok(updatedProduct);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted =
                await _productService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Product not found"
                });
            }

            return NoContent();
        }
    }
}