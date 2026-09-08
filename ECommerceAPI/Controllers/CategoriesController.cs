using ECommerceAPI.Data;
using ECommerceAPI.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // GET: api/categories
        // USER + ADMIN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }


        // =====================================================
        // GET: api/categories/1
        // USER + ADMIN
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            return Ok(category);
        }


        // =====================================================
        // POST: api/categories
        // ADMIN ONLY
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(
            Category category)
        {
            // Validate category name
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest(new
                {
                    message = "Category name is required"
                });
            }

            // Check duplicate category name
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == category.Name);

            if (existingCategory != null)
            {
                return BadRequest(new
                {
                    message = "Category already exists"
                });
            }

            // Create category
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                category);
        }


        // =====================================================
        // PUT: api/categories/1
        // ADMIN ONLY
        // =====================================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            Category category)
        {
            // Find existing category
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            // Validate name
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest(new
                {
                    message = "Category name is required"
                });
            }

            // Check duplicate name
            var duplicateCategory = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.Name == category.Name &&
                    c.Id != id);

            if (duplicateCategory != null)
            {
                return BadRequest(new
                {
                    message = "Another category with this name already exists"
                });
            }

            // Update category
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }


        // =====================================================
        // DELETE: api/categories/1
        // ADMIN ONLY
        // =====================================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Find category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            // Check whether products exist
            var hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return BadRequest(new
                {
                    message =
                        "Cannot delete category because products exist in this category"
                });
            }

            // Delete category
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}