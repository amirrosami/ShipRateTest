using FluentValidation;
using ShipRate.Domain.Entities.BaseInfo.Cargos;

namespace ShipRate.Application.Validations.Domain
{
    public class PackageTypesValidation: AbstractValidator<PackageType>
    {
        public PackageTypesValidation()
        {
            RuleFor(x=>x.PackageTypeCode).NotEmpty();
            RuleFor(x=>x.PackageTypeName).NotEmpty();
            RuleFor(x=>x.NumericCode).NotEmpty();
            RuleFor(x=>x.ENSCode).NotEmpty();
        }
    }


 }
