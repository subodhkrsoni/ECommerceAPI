using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemCreateDto> Items { get; set; }
            = new List<OrderItemCreateDto>();
    }
}