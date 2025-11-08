namespace FindFi.Domain.Entities;

public class ListingRef
{
    public long Id { get; set; } // Listing Id from external catalog (another DB)
    public long HostId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
}
