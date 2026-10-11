using FluentValidation;
using ShipRate.Domain.Entities.InventoryOperation;

namespace ShipRate.Application.Validations.Domain
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(x=>x.ProductCode).NotEmpty();
            RuleFor(x=>x.ProductName).NotEmpty();
            RuleFor(x => x.ProductCategoryId).NotEqual(Guid.Empty).WithMessage("Category is required");
        }
    }
 }
