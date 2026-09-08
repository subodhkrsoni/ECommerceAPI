namespace ECommerceAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<CartItem> CartItems { get; set; } = new();
    }
}