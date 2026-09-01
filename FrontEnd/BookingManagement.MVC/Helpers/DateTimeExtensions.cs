namespace BookingManagement.MVC.Helpers
{
    public static class DateTimeExtensions
    {
        // The API contract requires explicit UTC values. The UI labels every date/time field as UTC
        // and sends what the user typed unchanged, so no time zone conversion is applied here.
        public static DateTime AsUtc(this DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
