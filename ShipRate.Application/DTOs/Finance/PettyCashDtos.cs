using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Finance
{
    public sealed class PettyCashIndexSearchDto
    {
        public string? Term { get; set; }
        public int? CompanyId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class PettyCashListItemDto
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public decimal InitialAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public Guid? PersonId { get; set; }
        public string? PersonName { get; set; }
        public Guid? CustodianUserId { get; set; }
        public string? CustodianUserName { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class PettyCashUpsertDto : IValidatableObject
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required, StringLength(20)]
        public string Code { get; set; } = "";

        [Required, StringLength(50)]
        public string Title { get; set; } = "";

        [Range(0, double.MaxValue)]
        public decimal InitialAmount { get; set; }

        public decimal CurrentBalance { get; set; }

        public Guid? PersonId { get; set; }
        public Guid? CustodianUserId { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PersonId == null && CustodianUserId == null)
                yield return new ValidationResult("Person or Custodian User is required.", new[] { nameof(PersonId), nameof(CustodianUserId) });
        }
    }

    public sealed class PettyCashExpenseListItemDto
    {
        public Guid Id { get; set; }
        public Guid PettyCashId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string CategoryTitle { get; set; } = "";
        public string CostCenterTitle { get; set; } = "";
        public string Description { get; set; } = "";
        public int StatusId { get; set; }
        public string StatusName { get; set; } = "";
    }

    public sealed class PettyCashExpenseUpsertDto
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public Guid PettyCashId { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public Guid ExpenseCategoryId { get; set; }

        [Required]
        public Guid CostCenterId { get; set; }

        [Required, StringLength(200)]
        public string Description { get; set; } = "";

        public int? StatusId { get; set; } = 1;

        public decimal CurrentBalance { get; set; }
        public decimal? CategoryMaxAllowedAmount { get; set; }
    }

    public sealed class PettyCashRechargeListItemDto
    {
        public Guid Id { get; set; }
        public Guid PettyCashId { get; set; }
        public decimal RechargeAmount { get; set; }
        public DateTime? RechargeDate { get; set; }
        public string Description { get; set; } = "";
        public int StatusId { get; set; }
        public string StatusName { get; set; } = "";
    }

    public sealed class PettyCashRechargeUpsertDto
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public Guid PettyCashId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal RechargeAmount { get; set; }

        public DateTime? RechargeDate { get; set; } = DateTime.Today;

        [Required, StringLength(200)]
        public string Description { get; set; } = "";

        public int StatusId { get; set; } = 1;
    }

    public sealed class PettyCashLookupDto
    {
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Persons { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> ExpenseCategories { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
        public List<SelectListItem> ExpenseStatuses { get; set; } = new();
        public List<SelectListItem> RechargeStatuses { get; set; } = new();
    }
}
