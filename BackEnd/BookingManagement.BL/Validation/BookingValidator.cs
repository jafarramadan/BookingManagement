using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Exceptions;

namespace BookingManagement.BL.Validation
{
    public static class BookingValidator
    {
        public const int MaxIdentifierLength = 100;

        public static void ValidateCreateRequest(CreateBookingRequest request)
        {
            ValidateResourceId(request.ResourceId);
            ValidateIdentifier(request.UserId, nameof(request.UserId));
            EnsureUtc(request.StartDateTime, nameof(request.StartDateTime));
            EnsureUtc(request.EndDateTime, nameof(request.EndDateTime));

            if (request.StartDateTime >= request.EndDateTime)
            {
                throw new BusinessValidationException("'StartDateTime' must be earlier than 'EndDateTime'.");
            }
        }

        public static void ValidateResourceId(string? resourceId)
        {
            ValidateIdentifier(resourceId, nameof(CreateBookingRequest.ResourceId));
        }

        public static void ValidateBookingQuery(BookingQueryRequest query)
        {
            if (query.From.HasValue)
            {
                EnsureUtc(query.From.Value, nameof(query.From));
            }

            if (query.To.HasValue)
            {
                EnsureUtc(query.To.Value, nameof(query.To));
            }

            if (query.From.HasValue && query.To.HasValue && query.From.Value >= query.To.Value)
            {
                throw new BusinessValidationException("'from' must be earlier than 'to'.");
            }

            PagingValidator.Validate(query.Page, query.PageSize);
        }

        private static void ValidateIdentifier(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessValidationException($"'{fieldName}' is required.");
            }

            if (value.Trim().Length > MaxIdentifierLength)
            {
                throw new BusinessValidationException($"'{fieldName}' must not exceed {MaxIdentifierLength} characters.");
            }
        }

        private static void EnsureUtc(DateTime value, string fieldName)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                throw new BusinessValidationException($"'{fieldName}' must be an explicit UTC date/time, for example 2026-01-01T09:00:00Z.");
            }
        }
    }
}
