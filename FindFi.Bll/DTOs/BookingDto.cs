namespace FindFi.Bll.DTOs;

public class BookingDto
{
    public long Id { get; set; }
    public long ListingId { get; set; }
    public long GuestId { get; set; }
    public byte Status { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}