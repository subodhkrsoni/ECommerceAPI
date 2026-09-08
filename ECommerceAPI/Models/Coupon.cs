using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; } = "Percentage";

        [Range(0.01, 100000000)]
        public decimal DiscountValue { get; set; }

        [Range(0, 100000000)]
        public decimal MinimumOrderAmount { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}