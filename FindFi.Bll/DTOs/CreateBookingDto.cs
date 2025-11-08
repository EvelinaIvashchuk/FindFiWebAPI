using System.ComponentModel.DataAnnotations;

namespace FindFi.Bll.DTOs;

public class CreateBookingDto
{
    [Required]
    public long ListingId { get; set; }

    [Required]
    public long GuestId { get; set; }

    [Range(0, 3)]
    public byte Status { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Required, StringLength(10)]
    public string Currency { get; set; } = "USD";

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
}