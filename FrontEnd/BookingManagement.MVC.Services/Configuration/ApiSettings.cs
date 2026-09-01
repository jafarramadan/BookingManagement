namespace BookingManagement.MVC.Services.Configuration
{
    public class ApiSettings
    {
        public const string SectionName = "ApiSettings";

        public string BaseUrl { get; set; } = string.Empty;

        public int TimeoutSeconds { get; set; } = 30;
    }
}
