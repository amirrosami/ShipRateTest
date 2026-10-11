using FluentValidation;
using ShipRate.Domain.Entities.BaseInfo.Cargos;

namespace ShipRate.Application.Validations.Domain
{
    public class CountryHSCodeValidation : AbstractValidator<CountryHSCode>
    {
        public CountryHSCodeValidation()
        {
            RuleFor(x => x.CHSCode).NotEmpty();
            RuleFor(x => x.CHSName).NotEmpty();
            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage("Country is required");
            RuleFor(x => x.DutyRate).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }


 }
