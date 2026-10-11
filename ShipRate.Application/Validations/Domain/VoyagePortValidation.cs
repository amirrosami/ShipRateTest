using FluentValidation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Application.Validations.Domain
{
    public class VoyagePortValidation : AbstractValidator<VoyagePort>
    {
        public VoyagePortValidation()
        {
            RuleFor(x => x.InboundVoyageId).NotEqual(Guid.Empty).WithMessage("Voyage is Required");
            RuleFor(x => x.PortId).NotEqual(Guid.Empty).WithMessage("Port is Required");

        }
    }


 }
