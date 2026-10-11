using FluentValidation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Application.Validations.Domain
{
    public class VoyageValidation : AbstractValidator<Voyage>
        {
        public VoyageValidation()
        {
            RuleFor(x => x.VoyageNo).NotEmpty();
            RuleFor(x => x.DestinationPortId).NotEqual(Guid.Empty).WithMessage("Destination Port is Required");
            RuleFor(x => x.OriginPortId).NotEqual(Guid.Empty).WithMessage("Destination Port is Required");
        }
    }


 }
