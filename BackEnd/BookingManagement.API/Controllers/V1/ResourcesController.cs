using Asp.Versioning;
using BookingManagement.BL.Interfaces;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/resources")]
    [Produces("application/json")]
    public class ResourcesController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IResourceCatalog _resourceCatalog;

        public ResourcesController(IBookingService bookingService, IResourceCatalog resourceCatalog)
        {
            _bookingService = bookingService;
            _resourceCatalog = resourceCatalog;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ResourceDto>), StatusCodes.Status200OK)]
        public IActionResult GetResources()
        {
            return Ok(_resourceCatalog.GetAll());
        }

        [HttpGet("{resourceId}/bookings")]
        [ProducesResponseType(typeof(PagedResult<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetResourceBookings(
            string resourceId,
            [FromQuery] BookingQueryRequest query)
        {
            var bookings = await _bookingService.GetForResourceAsync(resourceId, query);

            return Ok(bookings);
        }
    }
}
