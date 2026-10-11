using FluentValidation;
using ShipRate.Domain.Entities.Agents;

namespace ShipRate.Application.Validations.Domain
{
    public class TerritoryCityValidation: AbstractValidator<TerritoryCity>
    {
        public TerritoryCityValidation()
        {
            RuleFor(x => x.CityId).NotEqual(Guid.Empty).WithMessage("City is reguired").NotNull().WithMessage("City is reguired");
        }
    }


 }
