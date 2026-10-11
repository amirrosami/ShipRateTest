using FluentValidation;
using ShipRate.Application.Utilities;
using ShipRate.Application.Validations.Custom;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Application.Validations.Domain
{
    public class AgentValidator : AbstractValidator<Agent>
    {
        public AgentValidator()
        {
        }
    }

    public class DriverValidator : AbstractValidator<Driver>
    {

        public DriverValidator()
        {
            RuleFor(d => d.LicenseNumber).NotEmpty();
            RuleFor(d => d.PassportNo).NotEmpty();
            RuleFor(d => d.SmartCardNo).NotEmpty();
        }
    }

    public class GPSProviderValidator : AbstractValidator<GPSProvider>
    {

        public GPSProviderValidator()
        {
            RuleFor(d => d.Id).NotNull();
        }
    }

    public class CarrierValidator : AbstractValidator<Carrier>
    {
        public CarrierValidator()
        {
            RuleFor(x => x.BookingEmail).EmailAddress();
            RuleFor(x => x.CarrierCode).NotEmpty();
            RuleFor(x => x.CarrierTypeId).GreaterThan(0);
            RuleFor(x => x.AWBPrefix).NotEmpty();
            RuleFor(x => x.RegistrationNumber).NotEmpty();
        }
    }
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.PersonalityTypeId).GreaterThan(0);
            When(x => x.PersonalityTypeId == 1 && x.CountryId == 1, () =>
            {
                RuleFor(x => x.NationalID)
                    .Must(NationalIdValidator.IsValidNationalId)
                    .WithMessage("NationalId is not valid.");
            });
            When(x => x.PersonalityTypeId == 2 && x.CountryId == 1, () =>
            {
                RuleFor(x => x.NationalID)
                    .Must(NationalCodeValidator.IsValidNationalCode)
                    .WithMessage("National Code is not valid.");
            });

            RuleFor(x => x.PersonalityTypeId).NotNull().NotEqual(0);
            RuleFor(x => x.PersonName).NotEmpty();
            RuleFor(x => x.PersonPersianName).NotEmpty();
            When(x => x.PersonalityTypeId == 2, () =>
            {
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.FirstName).NotEmpty();
            });
            RuleFor(x => x.EmailAddress).IsEmailOrEmpty();
            RuleFor(x => x.Phone).IsPhoneOrEmpty();
            RuleFor(x => x.Mobile).IsPhoneOrEmpty();
            RuleFor(x => x.FaxNo).EmptyOrDigitsOnly();
            RuleFor(x => x.ZipCode).EmptyOrDigitsOnly();
        }
    }


}
