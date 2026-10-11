using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class AccountCodeListItemVm
    {
        public Guid Id { get; set; }
        public string AccCode { get; set; } = string.Empty;
        public string BuyAccCode { get; set; } = string.Empty;
        public string SellAccCode { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public Guid? PersonId { get; set; }
        public string? PersonName { get; set; }

        public int? ChargeTypeId { get; set; }
        public string? ChargeTypeTitle { get; set; }
    }



    public class AccountCodeCreateEditVm : IValidatableObject
    {
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Party")]
        public Guid? PersonId { get; set; }

        [Display(Name = "Charge Type")]
        public int? ChargeTypeId { get; set; }

        [MaxLength(20)]
        [Display(Name = "Account Code")]
        public string AccCode { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Buy Account Code")]
        public string? BuyAccCode { get; set; }

        [MaxLength(20)]
        [Display(Name = "Sell Account Code")]
        public string? SellAccCode { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Persons { get; set; } = new();
        public List<SelectListItem> ChargeTypes { get; set; } = new();




        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasPerson = PersonId.HasValue;
            var hasCharge = ChargeTypeId.HasValue;

            if (!hasPerson && !hasCharge)
            {
                yield return new ValidationResult(
                    "Either Person or Charge Type must be selected.",
                    new[] { nameof(PersonId), nameof(ChargeTypeId) });
            }

            if (hasPerson && hasCharge)
            {
                yield return new ValidationResult(
                    "Only one of Person or Charge Type can be selected.",
                    new[] { nameof(PersonId), nameof(ChargeTypeId) });
            }
        }










    }

    public class AccountCodeSearchVm
    {
        public string? Search { get; set; }
        public int? CompanyId { get; set; }
        public int? ChargeTypeId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
