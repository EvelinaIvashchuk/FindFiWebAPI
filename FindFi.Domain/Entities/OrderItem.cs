namespace FindFi.Domain.Entities;

public class OrderItem
{
    public long OrderItemId { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
