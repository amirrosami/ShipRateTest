using FluentValidation;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Application.Validations.Domain
{
    public class EventTypesValidation : AbstractValidator<EventType>
    {
        public EventTypesValidation() {
            RuleFor(x=>x.EventTypeCode).NotEmpty();
            RuleFor(x=>x.EventTypeName).NotEmpty();
            RuleFor(x=>x.CategoryId).GreaterThan(0).WithMessage("Category is required");
        }
    }


 }
