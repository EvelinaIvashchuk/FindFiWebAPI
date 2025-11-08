namespace FindFi.Domain.Entities;

public class Payment
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public long? RentInvoiceId { get; set; }
    public byte Status { get; set; } // 0=pending,1=success,2=failed
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; }
}
