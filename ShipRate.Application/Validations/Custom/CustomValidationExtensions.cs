using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace ShipRate.Application.Validations.Custom
{
    public static class CustomValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> EmptyOrDigitsOnly<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(value =>
                    string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                .WithMessage("{PropertyName} must be empty or contain digits only.");
        }


        public static IRuleBuilderOptions<T, string> IsPhoneOrEmpty<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder.Must(value =>
                    string.IsNullOrEmpty(value) || (value.All(char.IsDigit)))
                .WithMessage("{PropertyName} is Invalid");
        }

        public static IRuleBuilderOptions<T, string> IsEmailOrEmpty<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder.Must(value =>
                    string.IsNullOrEmpty(value) || Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
                .WithMessage("{PropertyName} is Invalid");
        }

        public static IRuleBuilderOptions<T, string> IsPhone<T>(
        this IRuleBuilder<T, string> ruleBuilder)
        {
            //string.IsNullOrEmpty(value) || value.All(char.IsDigit)).Length(11)
            return ruleBuilder.Must(value =>
                    string.IsNullOrEmpty(value) || value.All(char.IsDigit))
                .WithMessage("{PropertyName} is Invalid");
        }
        public static string GetErrorsAsString(this ValidationResult validationResult)
        {

            if (validationResult == null || validationResult.IsValid)
            {
                return string.Empty;
            }
            string error = "";
            var errors = validationResult.Errors;
            foreach (var err in errors)
            {
                error += err.ErrorMessage + " - ";
            }
            return error;
        }
    }




}
