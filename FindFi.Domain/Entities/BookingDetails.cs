namespace FindFi.Domain.Entities;

public class BookingDetails
{
    public long Id { get; set; } // = Booking.Id
    public string? Notes { get; set; }
    public string? BillingEmail { get; set; }
}
