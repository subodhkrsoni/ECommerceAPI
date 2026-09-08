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
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // GET: api/orders
        // Login required
        // =====================================================

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,

                CustomerId = o.CustomerId,

                CustomerName = o.Customer != null
                    ? o.Customer.FirstName + " " + o.Customer.LastName
                    : null,

                TotalAmount = o.TotalAmount,

                Status = o.Status,

                CreatedAt = o.CreatedAt,

                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,

                    ProductId = oi.ProductId,

                    ProductName = oi.Product != null
                        ? oi.Product.Name
                        : null,

                    Quantity = oi.Quantity,

                    UnitPrice = oi.UnitPrice,

                    TotalPrice =
                        oi.Quantity * oi.UnitPrice

                }).ToList()

            }).ToList();

            return Ok(orderDtos);
        }


        // =====================================================
        // GET: api/orders/{id}
        // Login required
        // =====================================================

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,

                CustomerId = order.CustomerId,

                CustomerName = order.Customer != null
                    ? order.Customer.FirstName + " " + order.Customer.LastName
                    : null,

                TotalAmount = order.TotalAmount,

                Status = order.Status,

                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,

                    ProductId = oi.ProductId,

                    ProductName = oi.Product != null
                        ? oi.Product.Name
                        : null,

                    Quantity = oi.Quantity,

                    UnitPrice = oi.UnitPrice,

                    TotalPrice =
                        oi.Quantity * oi.UnitPrice

                }).ToList()
            };

            return Ok(orderDto);
        }


        // =====================================================
        // POST: api/orders
        // Login required
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            OrderCreateDto dto)
        {
            // 1. Check customer
            var customer = await _context.Customers
                .FindAsync(dto.CustomerId);

            if (customer == null)
            {
                return BadRequest(new
                {
                    message = "Customer not found"
                });
            }


            // 2. Check items
            if (dto.Items == null ||
                dto.Items.Count == 0)
            {
                return BadRequest(new
                {
                    message =
                        "Order must contain at least one item"
                });
            }


            // Get unique product IDs
            var productIds = dto.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();


            // Get products
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();


            // 3. Check products
            if (products.Count != productIds.Count)
            {
                return BadRequest(new
                {
                    message =
                        "One or more products not found"
                });
            }


            // 4. Check stock
            foreach (var item in dto.Items)
            {
                var product = products
                    .First(p =>
                        p.Id == item.ProductId);

                if (item.Quantity <= 0)
                {
                    return BadRequest(new
                    {
                        message =
                            "Quantity must be greater than zero"
                    });
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest(new
                    {
                        message =
                            $"Insufficient stock for product: {product.Name}"
                    });
                }
            }


            // 5. Calculate total
            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = products
                    .First(p =>
                        p.Id == item.ProductId);

                totalAmount +=
                    product.Price *
                    item.Quantity;
            }


            // 6. Create order
            var order = new Order
            {
                CustomerId = dto.CustomerId,

                TotalAmount = totalAmount,

                Status = "Pending",

                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);


            // 7. Create order items
            foreach (var item in dto.Items)
            {
                var product = products
                    .First(p =>
                        p.Id == item.ProductId);

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,

                    Quantity = item.Quantity,

                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);

                // Reduce stock
                product.Stock -= item.Quantity;
            }


            await _context.SaveChangesAsync();


            // 8. Return response
            var result = new OrderDto
            {
                Id = order.Id,

                CustomerId = order.CustomerId,

                CustomerName =
                    customer.FirstName +
                    " " +
                    customer.LastName,

                TotalAmount = order.TotalAmount,

                Status = order.Status,

                CreatedAt = order.CreatedAt,

                Items = order.OrderItems
                    .Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,

                        ProductId = oi.ProductId,

                        ProductName = products
                            .First(p =>
                                p.Id == oi.ProductId)
                            .Name,

                        Quantity = oi.Quantity,

                        UnitPrice = oi.UnitPrice,

                        TotalPrice =
                            oi.Quantity *
                            oi.UnitPrice

                    }).ToList()
            };


            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                result);
        }


        // =====================================================
        // PUT: api/orders/{id}/status
        // Admin only
        // =====================================================

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            OrderStatusUpdateDto dto)
        {
            // 1. Find order
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }


            // 2. Allowed statuses
            var allowedStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Shipped",
                "Delivered",
                "Cancelled"
            };


            // 3. Validate status name
            if (!allowedStatuses.Contains(
                    dto.Status,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid status. Allowed values: Pending, Confirmed, Shipped, Delivered, Cancelled"
                });
            }


            // 4. Normalize status
            var newStatus = allowedStatuses.First(
                s => s.Equals(
                    dto.Status,
                    StringComparison.OrdinalIgnoreCase));


            // 5. Current status
            var currentStatus = order.Status;


            // 6. Same status check
            if (currentStatus.Equals(
                    newStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message =
                        $"Order is already {currentStatus}"
                });
            }


            // 7. Validate status transition
            bool validTransition = currentStatus switch
            {
                "Pending" =>
                    newStatus == "Confirmed" ||
                    newStatus == "Cancelled",

                "Confirmed" =>
                    newStatus == "Shipped" ||
                    newStatus == "Cancelled",

                "Shipped" =>
                    newStatus == "Delivered",

                "Delivered" => false,

                "Cancelled" => false,

                _ => false
            };


            // 8. Invalid transition
            if (!validTransition)
            {
                return BadRequest(new
                {
                    message =
                        $"Cannot change order status from {currentStatus} to {newStatus}"
                });
            }


            // 9. Update status
            order.Status = newStatus;

            await _context.SaveChangesAsync();


            // 10. Success response
            return Ok(new
            {
                message =
                    "Order status updated successfully",

                orderId = order.Id,

                previousStatus = currentStatus,

                status = order.Status
            });
        }


        // =====================================================
        // DELETE: api/orders/{id}
        // Admin only
        // =====================================================

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(
            int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Order not found"
                });
            }


            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}