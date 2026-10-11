using FluentValidation;
using ShipRate.Application.Validations.Custom;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Infrastructure.Data.Configurations;

namespace ShipRate.Application.Validations.Domain
{


    public class LoactionValidator:AbstractValidator<Location>
    {
        public LoactionValidator()
        {

            

            RuleFor(x => x.LocationName).NotEmpty();
            RuleFor(x => x.LocationLocalName).NotEmpty();
            RuleFor(x => x.Phone).IsPhoneOrEmpty();
            RuleFor(x => x.LocationTypeId).GreaterThan(0).WithMessage("Location Type Is required");
            RuleFor(x => x.Email).IsEmailOrEmpty();
            RuleFor(x => x.CountryId).GreaterThan(0);
            When(x => x.LocationTypeId == 2, () => RuleFor(x => x.LocationCode).NotEmpty().WithMessage("Location Code is Required for City/Ports "));

            When(x => x.LocationTypeId == 2, () => RuleFor(x => x.CityTypeId).GreaterThan(0).WithMessage("City Type is Required for City/Ports "));

            When(x => x.LocationTypeId == 6, () => RuleFor(x => x.TerminalTypeId).GreaterThan(0).WithMessage("Terminal Type is Required for Terminals"));


            When(x => x.LocationTypeId == 4, () => RuleFor(x => x.CountryId2).GreaterThan(0).WithMessage("Country2 is Required for Border"));

            When(x => x.LocationTypeId == 4, () => RuleFor(x => x.CountryId2).GreaterThan(0).WithMessage("Country2 is Required for Border"));


            When(x => x.LocationTypeId == 7, () => RuleFor(x => x.DepotTypeId).GreaterThan(0).WithMessage("Depot Type is Required for Depot"));


            When(x => x.LocationTypeId == 3, () => RuleFor(x => x.IATACode).NotEmpty().WithMessage("IATACode is Required for Airport"));


            When(x => x.LocationTypeId == 3, () => RuleFor(x => x.ICAOCode).NotEmpty().WithMessage("ICAOCode is Required for Airport"));


        }
    }


 }
