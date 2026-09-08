namespace ECommerceAPI.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}