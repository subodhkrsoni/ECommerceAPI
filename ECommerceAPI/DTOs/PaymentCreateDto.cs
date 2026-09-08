using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class PaymentCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "UPI";

        public bool SimulateFailure { get; set; } = false;
    }
}