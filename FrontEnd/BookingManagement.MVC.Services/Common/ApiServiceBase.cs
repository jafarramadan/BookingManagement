using System.Net.Http.Json;
using System.Text.Json;
using BookingManagement.Common.Models;
using Microsoft.Extensions.Logging;

namespace BookingManagement.MVC.Services.Common
{
    public abstract class ApiServiceBase
    {
        protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        protected ApiServiceBase(HttpClient httpClient, ILogger logger)
        {
            HttpClient = httpClient;
            Logger = logger;
        }

        protected HttpClient HttpClient { get; }

        protected ILogger Logger { get; }

        protected async Task<ApiResult<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send)
        {
            try
            {
                using var response = await send();

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);

                    return data is null
                        ? ApiResult<T>.Failure((int)response.StatusCode, null)
                        : ApiResult<T>.Success(data);
                }

                var detail = await ReadProblemDetailAsync(response);

                Logger.LogWarning("The Booking Management API returned {StatusCode}: {Detail}", (int)response.StatusCode, detail);

                return ApiResult<T>.Failure((int)response.StatusCode, detail);
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
            {
                Logger.LogError(exception, "The Booking Management API could not be reached.");

                return ApiResult<T>.ConnectionFailure();
            }
        }

        protected static string FormatUtc(DateTime value) => Uri.EscapeDataString(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        private static async Task<string?> ReadProblemDetailAsync(HttpResponseMessage response)
        {
            try
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(JsonOptions);

                if (problemDetails is null)
                {
                    return null;
                }

                if (!string.IsNullOrWhiteSpace(problemDetails.Detail))
                {
                    return problemDetails.Detail;
                }

                var validationMessages = problemDetails.Errors?.SelectMany(error => error.Value).ToList();

                return validationMessages is { Count: > 0 }
                    ? string.Join(" ", validationMessages)
                    : problemDetails.Title;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
