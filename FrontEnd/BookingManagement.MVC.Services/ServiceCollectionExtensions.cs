using BookingManagement.MVC.Services.AuditLogs;
using BookingManagement.MVC.Services.Bookings;
using BookingManagement.MVC.Services.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookingManagement.MVC.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBookingManagementServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));

            services.AddHttpClient<IBookingAppService, BookingAppService>(ConfigureApiClient);
            services.AddHttpClient<IAuditLogAppService, AuditLogAppService>(ConfigureApiClient);

            return services;
        }

        private static void ConfigureApiClient(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;

            httpClient.BaseAddress = new Uri($"{apiSettings.BaseUrl.TrimEnd('/')}/");
            httpClient.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
        }
    }
}
