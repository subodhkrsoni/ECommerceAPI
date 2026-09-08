using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CartItemUpdateDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}