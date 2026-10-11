using FluentValidation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Application.Validations.Domain
{
    public class FlightValidation:AbstractValidator<Flight>
    {
        public FlightValidation()
        {
            RuleFor(x => x.AirLineId).NotNull().WithMessage("Airline is Required").NotEqual(Guid.Empty).WithMessage("Airline is Required");

            RuleFor(x => x.OriginAirportId).NotEqual(Guid.Empty).WithMessage("Origin Airport is Required");
            RuleFor(x => x.DestinationAirportId).NotEqual(Guid.Empty).WithMessage("Destination Airport is Required");

        }
    }


 }
