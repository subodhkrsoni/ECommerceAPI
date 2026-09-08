using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // GET: api/customers
        // LOGIN REQUIRED
        // =====================================================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            return Ok(customers);
        }


        // =====================================================
        // GET: api/customers/1
        // LOGIN REQUIRED
        // =====================================================
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            return Ok(customer);
        }


        // =====================================================
        // POST: api/customers
        // ADMIN ONLY
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCustomer(
            CustomerCreateDto dto)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (existingCustomer != null)
            {
                return BadRequest(new
                {
                    message = "Customer with this email already exists"
                });
            }


            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };


            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();


            return CreatedAtAction(
                nameof(GetCustomer),
                new { id = customer.Id },
                customer);
        }


        // =====================================================
        // PUT: api/customers/1
        // ADMIN ONLY
        // =====================================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            CustomerUpdateDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }


            // Check duplicate email
            var emailExists = await _context.Customers
                .AnyAsync(c =>
                    c.Email == dto.Email &&
                    c.Id != id);

            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Another customer already uses this email"
                });
            }


            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;


            await _context.SaveChangesAsync();


            return Ok(customer);
        }


        // =====================================================
        // DELETE: api/customers/1
        // ADMIN ONLY
        // =====================================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }


            // Check whether customer has orders
            var hasOrders = await _context.Orders
                .AnyAsync(o => o.CustomerId == id);

            if (hasOrders)
            {
                return BadRequest(new
                {
                    message = "Cannot delete customer because orders exist for this customer"
                });
            }


            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();


            return NoContent();
        }
    }
}