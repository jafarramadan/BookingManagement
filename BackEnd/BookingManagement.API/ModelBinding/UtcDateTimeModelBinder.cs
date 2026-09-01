using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookingManagement.API.ModelBinding
{
    // Query string dates are parsed with RoundtripKind so the caller's intent is preserved:
    // only a value carrying "Z" arrives as DateTimeKind.Utc. Anything else is rejected by the
    // business layer instead of being silently converted from server local time.
    public class UtcDateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                return Task.CompletedTask;
            }

            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedValue))
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    $"'{value}' is not a valid date/time. Use the ISO 8601 UTC format, for example 2026-01-01T09:00:00Z.");

                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(parsedValue);

            return Task.CompletedTask;
        }
    }

    public class UtcDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;

            if (modelType != typeof(DateTime) && modelType != typeof(DateTime?))
            {
                return null;
            }

            return new UtcDateTimeModelBinder();
        }
    }
}
