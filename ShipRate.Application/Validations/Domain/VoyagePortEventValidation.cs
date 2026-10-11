using FluentValidation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Application.Validations.Domain
{
    public class VoyagePortEventValidation : AbstractValidator<VoyagePortEvent>
    {
        public VoyagePortEventValidation()
        {
            RuleFor(x => x.EventTypeId).NotEqual(0).WithMessage("Event Type is Required");
        }
    }


 }
