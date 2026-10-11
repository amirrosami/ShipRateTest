using FluentValidation;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Application.Validations.Domain
{
    public class TripValidation : AbstractValidator<Trip>
    {
        public TripValidation()
        {


            RuleFor(x => x.ArrivalDate).Must((trip, arrivalDate) =>
            {
                if (!arrivalDate.HasValue || !trip.DepartureDate.HasValue)
                    return true;
                return arrivalDate.Value > trip.DepartureDate.Value;
            }).WithMessage("ArrivalDate must be greater than DepartureDate");


            RuleFor(x => x.ETA).Must((trip, eta) =>
            {
                if (!eta.HasValue || !trip.ETD.HasValue)
                    return true;
                return eta.Value > trip.ETD.Value;
            }).WithMessage("ETA must be greater than ETD");

            RuleFor(x => x.CarrierId).NotEqual(Guid.Empty).WithMessage("Carrier is required");
        }
    }
}
