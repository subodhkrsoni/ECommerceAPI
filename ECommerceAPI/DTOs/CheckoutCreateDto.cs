using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CheckoutCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        public string? CouponCode { get; set; }
    }
}