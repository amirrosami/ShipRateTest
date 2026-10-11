using FluentValidation;
using ShipRate.Domain.Entities.Finance;

namespace ShipRate.Application.Validations.Domain
{
    public class BankVaidation: AbstractValidator<Bank>
    {
        public BankVaidation()
        {
            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage("Country is required");
            RuleFor(x => x.BankCode).NotEmpty();
            RuleFor(x => x.BankName).NotEmpty();
            RuleFor(x => x.BankLocalName).NotEmpty();
        }
    }
 }
