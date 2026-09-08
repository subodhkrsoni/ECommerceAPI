using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICouponRepository _couponRepository;

        public CheckoutService(
            ApplicationDbContext context,
            ICouponRepository couponRepository)
        {
            _context = context;
            _couponRepository = couponRepository;
        }


        // ==========================================
        // Checkout
        // ==========================================

        public async Task<OrderDto?> CheckoutAsync(
            int userId,
            CheckoutCreateDto dto)
        {
            // ==========================================
            // Start Database Transaction
            // ==========================================

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // ==========================================
                // Get Current User's Cart
                // ==========================================

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c =>
                        c.UserId == userId);

                if (cart == null)
                {
                    return null;
                }


                // ==========================================
                // Check Empty Cart
                // ==========================================

                if (cart.CartItems == null ||
                    cart.CartItems.Count == 0)
                {
                    return null;
                }


                // ==========================================
                // Check Customer
                // ==========================================

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c =>
                        c.Id == dto.CustomerId);

                if (customer == null)
                {
                    return null;
                }


                // ==========================================
                // Get Product IDs From Cart
                // ==========================================

                var productIds = cart.CartItems
                    .Select(ci => ci.ProductId)
                    .Distinct()
                    .ToList();


                // ==========================================
                // Get Latest Products From Database
                // ==========================================

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();


                // ==========================================
                // Verify All Products Exist
                // ==========================================

                if (products.Count != productIds.Count)
                {
                    return null;
                }


                // ==========================================
                // Check Stock
                // ==========================================

                foreach (var cartItem in cart.CartItems)
                {
                    var product = products
                        .First(p =>
                            p.Id == cartItem.ProductId);

                    if (cartItem.Quantity <= 0)
                    {
                        return null;
                    }

                    if (product.Stock < cartItem.Quantity)
                    {
                        return null;
                    }
                }


                // ==========================================
                // Calculate Cart Total
                // ==========================================

                decimal totalAmount = 0;

                foreach (var cartItem in cart.CartItems)
                {
                    var product = products
                        .First(p =>
                            p.Id == cartItem.ProductId);

                    totalAmount +=
                        product.Price *
                        cartItem.Quantity;
                }


                // ==========================================
                // Apply Coupon
                // ==========================================

                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    var couponCode =
                        dto.CouponCode.Trim().ToUpper();

                    var coupon =
                        await _couponRepository
                            .GetByCodeAsync(couponCode);

                    // ------------------------------------------
                    // Coupon Exists?
                    // ------------------------------------------

                    if (coupon == null)
                    {
                        throw new Exception(
                            "Invalid coupon code.");
                    }


                    // ------------------------------------------
                    // Coupon Active?
                    // ------------------------------------------

                    if (!coupon.IsActive)
                    {
                        throw new Exception(
                            "Coupon is inactive.");
                    }


                    // ------------------------------------------
                    // Coupon Expired?
                    // ------------------------------------------

                    if (coupon.ExpiryDate <= DateTime.UtcNow)
                    {
                        throw new Exception(
                            "Coupon has expired.");
                    }


                    // ------------------------------------------
                    // Minimum Order Amount
                    // ------------------------------------------

                    if (totalAmount <
                        coupon.MinimumOrderAmount)
                    {
                        throw new Exception(
                            $"Minimum order amount for this coupon is {coupon.MinimumOrderAmount}.");
                    }


                    // ------------------------------------------
                    // Calculate Discount
                    // ------------------------------------------

                    decimal discountAmount = 0;

                    if (coupon.DiscountType
                        .Equals(
                            "Percentage",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        discountAmount =
                            totalAmount *
                            coupon.DiscountValue /
                            100;
                    }
                    else if (coupon.DiscountType
                        .Equals(
                            "Fixed",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        discountAmount =
                            coupon.DiscountValue;
                    }
                    else
                    {
                        throw new Exception(
                            "Invalid coupon discount type.");
                    }


                    // ------------------------------------------
                    // Discount Cannot Exceed Total
                    // ------------------------------------------

                    if (discountAmount > totalAmount)
                    {
                        discountAmount = totalAmount;
                    }


                    // ------------------------------------------
                    // Final Order Total
                    // ------------------------------------------

                    totalAmount -= discountAmount;
                }


                // ==========================================
                // Create Order
                // ==========================================

                var order = new Order
                {
                    CustomerId = dto.CustomerId,

                    TotalAmount = totalAmount,

                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);


                // ==========================================
                // Create Order Items
                // ==========================================

                foreach (var cartItem in cart.CartItems)
                {
                    var product = products
                        .First(p =>
                            p.Id == cartItem.ProductId);

                    var orderItem = new OrderItem
                    {
                        ProductId =
                            product.Id,

                        Quantity =
                            cartItem.Quantity,

                        UnitPrice =
                            product.Price
                    };

                    order.OrderItems.Add(orderItem);


                    // ==========================================
                    // Decrease Product Stock
                    // ==========================================

                    product.Stock -=
                        cartItem.Quantity;
                }


                // ==========================================
                // Remove Cart Items
                // ==========================================

                _context.CartItems.RemoveRange(
                    cart.CartItems);


                // ==========================================
                // Save Everything
                // ==========================================

                await _context.SaveChangesAsync();


                // ==========================================
                // Commit Transaction
                // ==========================================

                await transaction.CommitAsync();


                // ==========================================
                // Prepare Response
                // ==========================================

                var result = new OrderDto
                {
                    Id = order.Id,

                    CustomerId =
                        order.CustomerId,

                    CustomerName =
                        customer.FirstName +
                        " " +
                        customer.LastName,

                    TotalAmount =
                        order.TotalAmount,

                    Status =
                        order.Status,

                    CreatedAt =
                        order.CreatedAt,

                    Items =
                        order.OrderItems
                            .Select(oi =>
                            {
                                var product =
                                    products.First(p =>
                                        p.Id ==
                                        oi.ProductId);

                                return new OrderItemDto
                                {
                                    Id =
                                        oi.Id,

                                    ProductId =
                                        oi.ProductId,

                                    ProductName =
                                        product.Name,

                                    Quantity =
                                        oi.Quantity,

                                    UnitPrice =
                                        oi.UnitPrice,

                                    TotalPrice =
                                        oi.Quantity *
                                        oi.UnitPrice
                                };
                            })
                            .ToList()
                };

                return result;
            }
            catch
            {
                // ==========================================
                // Rollback On Error
                // ==========================================

                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}