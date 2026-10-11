using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.ChargeTypeVms
{
    public class ChargeTypeListViewModel
    {
        public string SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public List<ChargeTypeCategoryItem> Categories { get; set; }
        public List<ChargeTypeItem> ChargeTypes { get; set; }
    }

    public class ChargeTypeCategoryItem
    {
        public int Id { get; set; }
        public string ChargeTypeCategoryName { get; set; }
        public string ChargeTypeCategoryLocalName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class ChargeTypeItem
    {
        public int Id { get; set; }
        public string ChargeTypeCode { get; set; }
        public string? ChargeTypeCode2 { get; set; }
        public string ChargeTypeName { get; set; }
        public string ChargeTypeLocalName { get; set; }
        public string CategoryName { get; set; }
        public string CurrencyCode { get; set; }
        public string CalculationByName { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsActive { get; set; }
        public bool SystemDefined { get; set; }
    }

    public class ChargeTypeFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string ChargeTypeCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string ChargeTypeName { get; set; }

        [MaxLength(100)]
        public string ChargeTypeLocalName { get; set; }

        [Required]
        public int ChargeTypeCategoryId { get; set; }

        public bool IsTaxable { get; set; }
        public bool AllowManualAmount { get; set; }
        public bool AllowRateBased { get; set; }

        public int? CurrencyId { get; set; }

        [Required]
        public int CalculationById { get; set; }

        [Required]
        [MaxLength(20)]
        public string AccountingCode { get; set; }

        public bool AffectProfit { get; set; }
        public int? DependsOnChargeTypeId { get; set; }
        public decimal? MultiplierRate { get; set; }
        public bool IsActive { get; set; }

        [MaxLength(200)]
        public string Remarks { get; set; }

        public bool SystemDefined { get; set; }

        // Lookup lists
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public List<SelectListItem> CalculationByList { get; set; }
        public List<SelectListItem> ChargeTypesList { get; set; }
    }
}
