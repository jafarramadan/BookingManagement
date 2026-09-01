using BookingManagement.Common.Models;

namespace BookingManagement.MVC.Helpers
{
    // The service layer reports what happened (unreachable, status code, problem detail); choosing
    // the words the user reads is a presentation concern, so every message lives here once.
    public static class ApiFailureMessages
    {
        public const string ApiUnavailable = "Unable to connect to the Booking Management API. Please try again.";

        public static string Create(ApiResult result) => result switch
        {
            { IsConnectionFailure: true } => ApiUnavailable,
            { StatusCode: StatusCodes.Status409Conflict } => "This resource is already booked during the selected time.",
            { Detail: { Length: > 0 } detail } => detail,
            _ => "The booking could not be created. Please try again."
        };

        public static string Cancel(ApiResult result) => result switch
        {
            { IsConnectionFailure: true } => ApiUnavailable,
            { StatusCode: StatusCodes.Status404NotFound } => "This booking no longer exists.",
            { StatusCode: StatusCodes.Status409Conflict } => "This booking has already been cancelled.",
            { Detail: { Length: > 0 } detail } => detail,
            _ => "The booking could not be cancelled. Please try again."
        };

        public static string Query(ApiResult result, string fallback) => result switch
        {
            { IsConnectionFailure: true } => ApiUnavailable,
            { Detail: { Length: > 0 } detail } => detail,
            _ => fallback
        };
    }
}
