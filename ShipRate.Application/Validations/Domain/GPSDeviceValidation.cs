using FluentValidation;
using ShipRate.Domain.Entities.TransitOperation;

namespace ShipRate.Application.Validations.Domain
{
    public class GPSDeviceValidation : AbstractValidator<GPSDevice>
    {
        public GPSDeviceValidation()
        {
            RuleFor(x => x.GPSProviderId).NotEqual(Guid.Empty).WithMessage("Provider is Required");
            RuleFor(x => x.GPSDeviceStatusId).GreaterThan(0).WithMessage("Status is Required");
        }
    }
 }
