namespace FindFi.Domain.Entities;

public class Order
{
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public byte Status { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal TotalAmount { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation (not used by Dapper directly unless multi-mapping)
    public List<OrderItem> Items { get; set; } = new();
}
