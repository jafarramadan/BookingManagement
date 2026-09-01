using BookingManagement.Common.Exceptions;

namespace BookingManagement.BL.Validation
{
    public static class PagingValidator
    {
        public const int MaxPageSize = 200;

        public static void Validate(int page, int pageSize)
        {
            if (page < 1)
            {
                throw new BusinessValidationException("'page' must be greater than zero.");
            }

            if (pageSize < 1 || pageSize > MaxPageSize)
            {
                throw new BusinessValidationException($"'pageSize' must be between 1 and {MaxPageSize}.");
            }
        }
    }
}
