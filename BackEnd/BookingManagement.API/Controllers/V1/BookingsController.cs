using Asp.Versioning;
using BookingManagement.BL.Interfaces;
using BookingManagement.Common.DTOs.V1;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/bookings")]
    [Produces("application/json")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var booking = await _bookingService.CreateAsync(request);

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            return Ok(booking);
        }

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var booking = await _bookingService.CancelAsync(id);

            return Ok(booking);
        }
    }
}
