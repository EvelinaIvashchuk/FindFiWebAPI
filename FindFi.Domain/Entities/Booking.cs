namespace FindFi.Domain.Entities;

public class Booking
{
    public long Id { get; set; }
    public long ListingId { get; set; }
    public long GuestId { get; set; }
    public byte Status { get; set; } // 0=new,1=active,2=cancelled,3=completed
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
