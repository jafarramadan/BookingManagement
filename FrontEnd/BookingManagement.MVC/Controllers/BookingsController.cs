using BookingManagement.MVC.Helpers;
using BookingManagement.MVC.Models.Bookings;
using BookingManagement.MVC.Services.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.MVC.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingAppService _bookingAppService;

        public BookingsController(IBookingAppService bookingAppService)
        {
            _bookingAppService = bookingAppService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [Bind(Prefix = BookingSearchViewModel.QueryPrefix)] BookingSearchViewModel search)
        {
            var dashboard = await BuildDashboardAsync(new CreateBookingViewModel(), search);

            return View(dashboard);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind(Prefix = "create")] CreateBookingViewModel create,
            [Bind(Prefix = BookingSearchViewModel.QueryPrefix)] BookingSearchViewModel search)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), await BuildDashboardAsync(create, search));
            }

            var result = await _bookingAppService.CreateAsync(create.ToDto());

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, ApiFailureMessages.Create(result));

                return View(nameof(Index), await BuildDashboardAsync(create, search));
            }

            TempData["SuccessMessage"] = "Booking created successfully.";

            return RedirectToAction(nameof(Index), search.ToRouteValues());
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(
            Guid id,
            [Bind(Prefix = BookingSearchViewModel.QueryPrefix)] BookingSearchViewModel search)
        {
            var result = await _bookingAppService.CancelAsync(id);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = ApiFailureMessages.Cancel(result);
            }

            return RedirectToAction(nameof(Index), search.ToRouteValues());
        }

        private async Task<DashboardViewModel> BuildDashboardAsync(
            CreateBookingViewModel create,
            BookingSearchViewModel search)
        {
            var dashboard = new DashboardViewModel
            {
                Create = create,
                Search = search
            };

            if (search.HasResourceId)
            {
                await LoadBookingsAsync(search);
            }

            return dashboard;
        }

        private async Task LoadBookingsAsync(BookingSearchViewModel search)
        {
            var result = await _bookingAppService.GetForResourceAsync(search.ToInputDto());

            if (!result.IsSuccess || result.Data is null)
            {
                search.ErrorMessage = ApiFailureMessages.Query(result, "The bookings could not be loaded.");

                return;
            }

            search.Bookings = result.Data.Items.Select(BookingRowViewModel.FromDto).ToList();
            search.ApplyPaging(result.Data);
        }
    }
}
