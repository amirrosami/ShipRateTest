using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace ShipRate.Application.Utilities
{
    public class CustomNumericModelBinder : IModelBinder
    {
        private readonly Type _targetType;

        public CustomNumericModelBinder(Type targetType)
        {
            _targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var rawValue = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return Task.CompletedTask;
            }

            // حذف کامل کاما و فاصله‌ها
            var cleanValue = rawValue.Replace(",", "").Trim();

            try
            {
                object? result = null;

                if (_targetType == typeof(long))
                {
                    if (long.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                        result = val;
                }
                else if (_targetType == typeof(int))
                {
                    if (int.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                        result = val;
                }
                else if (_targetType == typeof(decimal))
                {
                    if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                        result = val;
                }
                else if (_targetType == typeof(double))
                {
                    if (double.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                        result = val;
                }

                if (result != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
                else
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"The value '{rawValue}' is not valid.");
                }
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"The value '{rawValue}' is not valid.");
            }

            return Task.CompletedTask;
        }
    }

    public class CustomNumericModelBinderProvider : IModelBinderProvider
    {
        private static readonly HashSet<Type> SupportedTypes = new()
    {
        typeof(long), typeof(long?),
        typeof(int), typeof(int?),
        typeof(decimal), typeof(decimal?),
        typeof(double), typeof(double?)
    };

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (SupportedTypes.Contains(context.Metadata.ModelType))
            {
                return new CustomNumericModelBinder(context.Metadata.ModelType);
            }
            return null;
        }
    }





}
