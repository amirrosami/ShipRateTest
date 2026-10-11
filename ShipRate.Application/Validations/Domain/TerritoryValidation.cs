using FluentValidation;
using ShipRate.Domain.Entities.Agents;

namespace ShipRate.Application.Validations.Domain
{
    public class TerritoryValidation: AbstractValidator<Territory>
    {
        public TerritoryValidation()
        {
            RuleFor(c => c.TerritoryName).NotEmpty().NotNull();

        }
    }


 }
