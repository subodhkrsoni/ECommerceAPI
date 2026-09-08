using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CouponUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "Percentage";

        [Range(0.01, 100000000)]
        public decimal DiscountValue { get; set; }

        [Range(0, 100000000)]
        public decimal MinimumOrderAmount { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }
    }
}