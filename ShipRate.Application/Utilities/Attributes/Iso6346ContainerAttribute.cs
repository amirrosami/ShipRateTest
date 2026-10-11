using ShipRate.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class Iso6346ContainerAttribute : ValidationAttribute
    {
        public Iso6346ContainerAttribute()
            : base("The entered container number is not a valid ISO 6346 number.")
        {
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // بررسی خالی نبودن برعهده [Required] است
                return ValidationResult.Success;
            }
            var containerNumber = value.ToString()!.Trim().ToUpperInvariant();
            if (!Iso6346.ValidateContainerNumber(containerNumber))
            {
                return new ValidationResult(
                    $"Container number '{containerNumber}' is not a valid ISO 6346 number.",
                    new[] { validationContext.MemberName! }
                );
            }
            return ValidationResult.Success;
        }
    }
}
