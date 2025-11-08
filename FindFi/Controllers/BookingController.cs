using FindFi.Bll.Abstractions;
using FindFi.Bll.DTOs;
using FindFi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FindFi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await bookingService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await bookingService.GetByIdAsync(id, cancellationToken);
            return Ok(item);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var id = await bookingService.CreateAsync(dto, cancellationToken);
            var created = await bookingService.GetByIdAsync(id, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }
        catch (ValidationException ex)
        {
            foreach (var kv in ex.Errors)
            {
                foreach (var msg in kv.Value)
                {
                    ModelState.AddModelError(kv.Key, msg);
                }
            }
            return ValidationProblem(ModelState);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            await bookingService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            foreach (var kv in ex.Errors)
            {
                foreach (var msg in kv.Value)
                {
                    ModelState.AddModelError(kv.Key, msg);
                }
            }
            return ValidationProblem(ModelState);
        }
        catch (BusinessConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await bookingService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}