using FluentValidation;
using ShipRate.Domain.Entities.BaseInfo.Cargos;

namespace ShipRate.Application.Validations.Domain
{
    public class HSCodeValidation : AbstractValidator<HSCode>
    {
        public HSCodeValidation()
        {
            RuleFor(x => x.HSCodeCode).NotEmpty();
            RuleFor(x => x.HSCodeName).NotEmpty();
            RuleFor(x => x.HSLevelId).GreaterThan(0).WithMessage("HSLevel is Required");
        }
    }


 }
