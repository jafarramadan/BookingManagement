namespace BookingManagement.MVC.Models.Bookings
{
    public class DashboardViewModel
    {
        public CreateBookingViewModel Create { get; set; } = new();

        public BookingSearchViewModel Search { get; set; } = new();

        public IDictionary<string, string> ToRouteData(int? page = null) => Search.ToRouteData(page);
    }
}
