using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CartItemAddDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}