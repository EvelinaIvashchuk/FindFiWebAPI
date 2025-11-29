using FindFi.Bll.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FindFi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MetricsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("booking-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetBookingCount(CancellationToken cancellationToken)
    {
        var items = await bookingService.GetCountAsync(cancellationToken);
        return Ok(items);
    }
}