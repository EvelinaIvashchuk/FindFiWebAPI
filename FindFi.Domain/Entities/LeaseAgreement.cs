namespace FindFi.Domain.Entities;

public class LeaseAgreement
{
    public long Id { get; set; } // = Booking.Id
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal DepositAmount { get; set; } = 0m;
    public string Currency { get; set; } = "USD";
    public string? Terms { get; set; }
}
