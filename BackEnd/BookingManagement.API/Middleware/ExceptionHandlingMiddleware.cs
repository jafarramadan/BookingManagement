using BookingManagement.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await WriteProblemDetailsAsync(context, exception);
            }
        }

        private async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
        {
            var problemDetails = BuildProblemDetails(context, exception);

            if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("{Title} while processing {Method} {Path}: {Message}",
                    problemDetails.Title, context.Request.Method, context.Request.Path, exception.Message);
            }

            context.Response.Clear();
            context.Response.StatusCode = problemDetails.Status!.Value;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private static ProblemDetails BuildProblemDetails(HttpContext context, Exception exception)
        {
            var (statusCode, title, detail) = exception switch
            {
                BusinessValidationException => (StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
                NotFoundException => (StatusCodes.Status404NotFound, "Resource not found", exception.Message),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected error", "An unexpected error occurred while processing the request.")
            };

            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
        }
    }
}
