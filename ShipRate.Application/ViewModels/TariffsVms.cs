using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class TariffTypeVm
    {
        public int Id { get; set; }
        public string TariffTypeName { get; set; }
        public string TariffTypeLocalName { get; set; }
    }
    public class TariffBookSearchVm
    {
        public string? SearchText { get; set; }

        public string? PartyName { get; set; }

        public int? TariffTypeId { get; set; }

        public Guid? PartyId { get; set; }

        public int? ChargeTypeId { get; set; }

        public int? StatusId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public List<SelectListItem> TariffTypes { get; set; } = new();
        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

    public class TariffBookListItemVm
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int TariffTypeId { get; set; }

        public string TariffTypeName { get; set; } = null!;

        public Guid? PartyId { get; set; }

        public string? PartyName { get; set; }

        public int? CurrencyId { get; set; }

        public string? CurrencyName { get; set; }

        public int? StatusId { get; set; }

        public string? StatusName { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; set; }
    }


    public class TariffRuleListItemVm
    {
        public Guid Id { get; set; }
        public Guid TariffBookId { get; set; }
        public int ChargeTypeId { get; set; }
        public string ChargeTypeName { get; set; } = null!;
        public int? CalculationById { get; set; }
        public string? CalculationByName { get; set; }
        public int? CalculationTypeId { get; set; }
        public string? CalculationTypeName { get; set; }
        public decimal? FromValue { get; set; }
        public decimal? ToValue { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }

        public string ContainerSizeTypeName { get; set; }
        public string OriginName { get; set; }
        public string DestinationName { get; set; }

        public string TradeTypeName { get; set; }
        public string FullMTName { get; set; }
        public bool IsDG { get; set; }
        public bool IsOverSized { get; set; }
        public bool IsOverWeight { get; set; }




        public bool HasCoverageGrid => ChargeTypeId == 555;
    }

    public class TariffRuleCoverageListItemVm
    {
        public Guid Id { get; set; }

        public Guid TariffRuleId { get; set; }

        public int? CountryId { get; set; }

        public string? CountryName { get; set; }

        public int? LocationId { get; set; }

        public string? LocationName { get; set; }

        public string? CoverageCode { get; set; }

        public string? CoverageName { get; set; }

        public bool IsActive { get; set; }
    }

    public class TariffBookFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int TariffTypeId { get; set; }

        public Guid? PartyId { get; set; }

        public int? CurrencyId { get; set; }

        public string? PartyDisplayName { get; set; }

        public int? StatusId { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; } = DateTime.Today;

        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; set; } = true;

        public List<SelectListItem> TariffTypes { get; set; } = new();

        public List<SelectListItem> Statuses { get; set; } = new();

        public List<SelectListItem> Currencies { get; set; } = new();
    }

    public class TariffRuleFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid TariffBookId { get; set; }

        [Required]
        public int ChargeTypeId { get; set; }

        public int? CalculationById { get; set; }

        public int? CalculationTypeId { get; set; }

        public decimal? FromValue { get; set; }

        public decimal? ToValue { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public int? CurrencyId { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; } = DateTime.Today;

        public DateTime? EffectiveTo { get; set; }

        //public int Priority { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ExtraAttributes { get; set; }

        public int? ContainerSizeId { get; set; }
        public int? ContainerSizeTypeId { get; set; }

        public List<SelectListItem> ChargeTypes { get; set; } = new();

        public List<SelectListItem> CalculationByItems { get; set; } = new();

        public List<SelectListItem> CalculationTypes { get; set; } = new();

        public List<SelectListItem> Currencies { get; set; } = new();

        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();
        public List<SelectListItem> ContainerSizes { get; set; } = new();
    }

    public class TariffRuleCoverageFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid TariffRuleId { get; set; }

        public int? CountryId { get; set; }

        public int? LocationId { get; set; }

        [MaxLength(50)]
        public string? CoverageCode { get; set; }

        [MaxLength(200)]
        public string? CoverageName { get; set; }

        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Countries { get; set; } = new();

        public List<SelectListItem> Locations { get; set; } = new();
    }

    public class DeleteConfirmVm
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "Delete Confirmation";

        public string Message { get; set; } = "Are you sure you want to delete this item?";

        public string PostUrl { get; set; } = null!;

        public string RefreshTarget { get; set; } = null!;

        public string RefreshUrl { get; set; } = null!;
    }

    public class PartyLookupItemVm
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Mobile { get; set; }
    }


    public class LocationLookupItemVm
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = null!;
    }

    public class BLLookupItemVm
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = null!;
    }



    public class PackageTypeLookupItemVm
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = null!;
    }


    public class HSCodeLookupItemVm
    {
        public string HSCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }



    public class TariffIndexVm
    {
        public TariffBookSearchVm Search { get; set; } = new();
        public List<SelectListItem> TariffTypes { get; set; } = new();
        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public PagedResultDto<TariffBookListItemVm> TariffBooks { get; set; } = new();
    }
    public class HtmxModalSuccessVm
    {
        public string RefreshTarget { get; set; } = null!;

        public string RefreshUrl { get; set; } = null!;

        public string[]? ClearTargets { get; set; }
    }
    public class PagerVm
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public string Target { get; set; } = null!;

        public string Url { get; set; } = null!;

        public string? IncludeFormSelector { get; set; }
    }




    public class DemurrageRuleBatchFormVm
    {
        public Guid TariffBookId { get; set; }
        public int? ContainerSizeTypeId { get; set; }

        public string? TariffBookTitle { get; set; }
        public string? CurrencyTitle { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public List<DemurrageRuleStepVm> Steps { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();
    }
    public class DemurrageRuleStepVm
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Amount { get; set; }
    }






    public class OceanFreightRuleBatchFormVm
    {
        public Guid TariffBookId { get; set; }
        public int TariffTypeId { get; set; } = 1;

        public string? TariffBookTitle { get; set; }
        public string? CurrencyTitle { get; set; }

        public int? OriginCountryId { get; set; }
        public int? DestinationCountryId { get; set; }

        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }
        public Guid? PartyId { get; set; }
        public int? ScopeTypeId { get; set; }

        public int? CurrencyId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public string? PastedRows { get; set; }

        public List<SelectListItem> OriginCountries { get; set; } = new();
        public List<SelectListItem> DestinationCountries { get; set; } = new();
        public List<SelectListItem> Origins { get; set; } = new();
        public List<SelectListItem> Destinations { get; set; } = new();
        public List<SelectListItem> Parties { get; set; } = new();
        public List<SelectListItem> ScopeTypes { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();
        public List<SelectListItem> ChargeTypes { get; set; } = new();

        public List<OceanFreightRateRowVm> Rows { get; set; } = new();

        public string? CopiedRowsText { get; set; }
        public string? PastedRowsText { get; set; }


    }

    public class OceanFreightRateRowVm
    {
        public int? ContainerSizeTypeId { get; set; }
        public int? ChargeTypeId { get; set; }
        public decimal? Amount { get; set; }
        public string? ExtraAttributes { get; set; }
    }
    public class TariffThcRuleBatchFormVm
    {
        public Guid TariffBookId { get; set; }
        public string? TariffBookTitle { get; set; }

        public Guid? LocationId { get; set; } // Port / Terminal
        public string? LocationName { get; set; }
        public int? CurrencyId { get; set; }
        public int ChargeTypeId { get; set; } = 155; // Default THC Charge Type

        // Shared Dropdowns for form and rows
        public List<SelectListItem> FullMTs { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();

        // Rows for Batch Entry
        public List<TariffThcRuleRowVm> Rows { get; set; } = new();
    }

    public class TariffThcRuleRowVm
    {
        public Guid? Id { get; set; }

        // Row-Level Trade Type (Import, Export, Transit, etc.)
        public int? TradeTypeId { get; set; }
        public string? TradeTypeName { get; set; }

        public int? ContainerSizeTypeId { get; set; }
        public string? ContainerSizeTypeName { get; set; }

        public int? FullMTId { get; set; }
        public string? FullMTName { get; set; }

        public bool IsDG { get; set; }
        public bool IsOverSized { get; set; }
        public bool IsOverWeight { get; set; }

        public decimal Amount { get; set; }
        public string? Remarks { get; set; }
    }

    public class LocationLookupVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class TariffRuleForSaoiVM
    {
        public Guid TariffBookId { get; set; }

        public int Row1TempFeeValue { get; set; }

        public int Row2_Issue_1_From { get; set; }
        public int Row2_Issue_1_To { get; set; }
        public int Row2_Issue_1_Value { get; set; }

        public int Row2_Issue_2_From { get; set; }
        public int Row2_Issue_2_To { get; set; }
        public int Row2_Issue_2_Value { get; set; }

        public int Row2_Issue_3_From { get; set; }
        public int Row2_Issue_3_To { get; set; }
        public int Row2_Issue_3_Value { get; set; }



        public int Row3_AmmendAdmin_Value { get; set; }

        public int Row4_MergeManifest_Value { get; set; }

        public int Row5_TranshipValue_Value { get; set; }

        public int Row6_Upload_Value { get; set; }

        public int Row7_Seal_Value { get; set; }

        public int Row8_labelRemoval_Value { get; set; }

        public int Row9_Danger_Value { get; set; }

        public int Row10_ExportCancel_Value { get; set; }

        public int Row11_Shifting_Value { get; set; }

        public int Row12_Extra_HsCode_Value { get; set; }


    }

























}
