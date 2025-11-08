namespace FindFi.Domain.Entities;

public class RentInvoice
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    public decimal Amount { get; set; }
    public byte Status { get; set; } // 0=pending,1=paid,2=overdue
    public DateTime CreatedAt { get; set; }
}
