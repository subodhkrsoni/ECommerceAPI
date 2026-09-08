using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = "UPI";

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}