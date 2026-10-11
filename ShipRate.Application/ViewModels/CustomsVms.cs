using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class CustomsClearanceCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Job is required.")]
        public Guid JobId { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Declaration Type is required.")]
        public int? DeclarationTypeId { get; set; }

        [Required(ErrorMessage = "Custom Office is required.")]
        public Guid? CustomOfficeId { get; set; }

        [Required(ErrorMessage = "Importer/Exporter is required.")]
        public Guid? ImporterExporterId { get; set; }

        [Required(ErrorMessage = "Customs Broker is required.")]
        public Guid? CustomsBrokerId { get; set; }

        [MaxLength(100)]
        public string? DeclarationNumber { get; set; }

        public DateTime? DeclarationDate { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public IEnumerable<SelectListItem> DeclarationTypes { get; set; } = Array.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> CustomOffices { get; set; } = Array.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ImporterExporters { get; set; } = Array.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Persons { get; set; } = Array.Empty<SelectListItem>();
    }
    public class CustomsClearanceJobTabVm
    {
        public Guid JobId { get; set; }
        public List<CustomsClearanceListItemVm> Items { get; set; } = new();
    }



    public class CustomsClearanceListItemVm
    {
        public Guid Id { get; set; }
        public string? DeclarationNumber { get; set; }
        public string DeclarationTypeTitle { get; set; } = null!;
        public string CustomOfficeName { get; set; } = null!;
        public string ImporterExporterName { get; set; } = null!;
        public DateTime? DeclarationDate { get; set; }
        public string StatusTitle { get; set; } = null!;
        public bool CanDelete { get; set; }
    }

    public class CustomsClearanceDetailsVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string? JobNumber { get; set; }
        public string? DeclarationNumber { get; set; }
        public string? StatusTitle { get; set; }
        public string? ImporterExporterName { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
    }


    public class CustomsClearanceItemCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid CustomsClearanceId { get; set; }

        [Display(Name = "Item Seq.")]
        [Range(1, int.MaxValue)]
        public int ItemSequence { get; set; }

        [Display(Name = "Country HS Code")]
        [Required]
        public Guid CountryHSCodeId { get; set; }

        [Display(Name = "HS Code")]
        [Required]
        [MaxLength(50)]
        public string HSCodeToSave { get; set; } = string.Empty;

        [Display(Name = "Goods Description (EN)")]
        [Required]
        [MaxLength(1000)]
        public string GoodsDescriptionEN { get; set; } = string.Empty;

        [Display(Name = "Goods Description (Local)")]
        [MaxLength(1000)]
        public string? GoodsDescriptionLocal { get; set; }

        [Display(Name = "Quantity")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal? Quantity { get; set; }

        [Display(Name = "Measurement Unit")]
        public int? MeasurementUnitId { get; set; }

        [Display(Name = "Statistical Quantity")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal? StatisticalQuantity { get; set; }

        [Display(Name = "Statistical Unit")]
        public int? StatisticalMeasurementUnitId { get; set; }

        [Display(Name = "Unit Price")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        [Display(Name = "Total Value")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal? TotalValue { get; set; }

        [Display(Name = "Remarks")]
        [MaxLength(1000)]
        public string? Remarks { get; set; }

        // Lookups
        public List<SelectListItem> CountryHsCodes { get; set; } = new();
        public List<SelectListItem> MeasurementUnits { get; set; } = new();
        public List<SelectListItem> StatisticalMeasurementUnits { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
    }
    public class CustomsClearanceItemListItemVm
    {
        public Guid Id { get; set; }
        public int ItemSequence { get; set; }
        public string HSCodeToSave { get; set; } = string.Empty;
        public string GoodsDescriptionEN { get; set; } = string.Empty;
        public string? GoodsDescriptionLocal { get; set; }

        public decimal? Quantity { get; set; }
        public string? MeasurementUnitName { get; set; }

        public decimal? UnitPrice { get; set; }
        public string? CurrencyCode { get; set; }

        public decimal? TotalValue { get; set; }
        public decimal TotalDutiesItem { get; set; }
    }
    public class CustomsClearanceItemsTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public List<CustomsClearanceItemListItemVm> Items { get; set; } = new();
    }

    public class CustomsClearanceContainerCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid CustomsClearanceId { get; set; }

        [Display(Name = "Container")]
        public Guid? ContainerId { get; set; }

        [Display(Name = "Container No")]
        [Required]
        [MaxLength(50)]
        public string ContainerNo { get; set; } = string.Empty;

        [Display(Name = "Size / Type")]
        [Required]
        public int ContainerSizeTypeId { get; set; }

        [Display(Name = "Full Gate-In Ref")]
        public Guid? FullGateInContainerId { get; set; }

        [Display(Name = "VGM Ref")]
        public Guid? VGMContainerId { get; set; }

        [Display(Name = "Seal No")]
        [MaxLength(100)]
        public string? SealNo { get; set; }

        [Display(Name = "Package Type")]
        public int? PackageTypeId { get; set; }

        [Display(Name = "Gross Weight (Kg)")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal GrossWeightKg { get; set; }

        [Display(Name = "Net Weight (Kg)")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal NetWeightKg { get; set; }

        [Display(Name = "Remarks")]
        [MaxLength(1000)]
        public string? Remarks { get; set; }

        // Lookups
        public List<SelectListItem> Containers { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();
        public List<SelectListItem> FullGateInContainers { get; set; } = new();
        public List<SelectListItem> VgmContainers { get; set; } = new();
        public List<SelectListItem> PackageTypes { get; set; } = new();
    }

    public class CustomsClearanceContainerListItemVm
    {
        public Guid Id { get; set; }
        public string ContainerNo { get; set; } = string.Empty;
        public string ContainerSizeTypeName { get; set; } = string.Empty;
        public string? SealNo { get; set; }
        public string? PackageTypeName { get; set; }
        public decimal GrossWeightKg { get; set; }
        public decimal NetWeightKg { get; set; }
        public string? Remarks { get; set; }
    }

    public class CustomsClearanceContainersTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public List<CustomsClearanceContainerListItemVm> Containers { get; set; } = new();
    }



    public class CustomsClearanceChargeCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid CustomsClearanceId { get; set; }

        [Display(Name = "Related Item")]
        public Guid? CustomsClearanceItemId { get; set; }

        [Display(Name = "Charge Type")]
        [Required]
        public int ChargeTypeId { get; set; }

        [Display(Name = "Description")]
        [Required]
        [MaxLength(500)]
        public string ChargeDescription { get; set; } = string.Empty;

        [Display(Name = "Amount")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Currency")]
        [Required]
        public int CurrencyId { get; set; }

        [Display(Name = "Exchange Rate")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal ExchangeRate { get; set; } = 1;

        [Display(Name = "Local Amount")]
        [Range(typeof(decimal), "0", "999999999999")]
        public decimal LocalAmount { get; set; }

        [Display(Name = "Tax")]
        public bool IsTax { get; set; }

        public bool IsSystemCalculated { get; set; }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }

        [Display(Name = "Paid At")]
        [DataType(DataType.Date)]
        public DateTime? PaidAt { get; set; }

        [Display(Name = "Payment Ref. No")]
        [MaxLength(100)]
        public string? PaymentReferenceNo { get; set; }

        [Display(Name = "Invoice No")]
        [MaxLength(100)]
        public string? InvoiceNo { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime? InvoiceDate { get; set; }

        [Display(Name = "Remarks")]
        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> ClearanceItems { get; set; } = new();
    }
    public class CustomsClearanceChargeListItemVm
    {
        public Guid Id { get; set; }
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeDescription { get; set; } = string.Empty;
        public string? ItemReference { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyName { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; }
        public decimal LocalAmount { get; set; }
        public bool IsTax { get; set; }
        public bool IsSystemCalculated { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentReferenceNo { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class CustomsClearanceChargesTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public List<CustomsClearanceChargeListItemVm> Charges { get; set; } = new();
    }



    public class CustomsClearanceTimelineTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public List<ActivityLogListItemVm> Entries { get; set; } = new();
    }



    public class CustomsClearanceLicenseCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid CustomsClearanceId { get; set; }

        [Display(Name = "License Type")]
        [Required]
        public int? LicenseTypeId { get; set; }

        [Display(Name = "Issuing Organization")]
        [StringLength(200)]
        public string? IssuingOrganization { get; set; }

        [Display(Name = "License Number")]
        [StringLength(100)]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Issue Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Expiry Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "License Status")]
        [Required]
        public int? LicenseStatusId { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(1000)]
        public string? Remarks { get; set; }

        public bool IsEdit { get; set; }
        public bool CanEdit { get; set; }

        public List<SelectListItem> LicenseTypeLookup { get; set; } = new();
        public List<SelectListItem> LicenseStatusLookup { get; set; } = new();
    }

    public class CustomsClearanceLicenseListItemVm
    {
        public Guid Id { get; set; }
        public Guid CustomsClearanceId { get; set; }

        public string LicenseTypeName { get; set; } = string.Empty;
        public string? IssuingOrganization { get; set; }
        public string? LicenseNumber { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public int LicenseStatusId { get; set; }
        public string LicenseStatusName { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
        public int DaysToExpiry { get; set; }

        public bool CanEdit { get; set; }
    }
    public class CustomsClearanceLicensesTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public bool CanEdit { get; set; }

        public List<CustomsClearanceLicenseListItemVm> Licenses { get; set; } = new();
    }



    public class CustomsInspectionCreateEditVm
    {
        public Guid? Id { get; set; }
        public Guid CustomsClearanceId { get; set; }

        [Display(Name = "Inspection Type")]
        [Required]
        public int? InspectionTypeId { get; set; }

        [Display(Name = "Inspection Method")]
        [Required]
        public int? InspectionMethodId { get; set; }

        [Display(Name = "Inspection Result")]
        public int? InspectionResultId { get; set; }

        [Display(Name = "Inspection Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? InspectionDate { get; set; }

        [Display(Name = "Inspector Name")]
        public string? InspectorName { get; set; }

        public string? Location { get; set; }
        public string? Findings { get; set; }

        [Display(Name = "Discrepancy Found?")]
        public bool DiscrepancyFound { get; set; }
        public string? DiscrepancyDescription { get; set; }

        [Display(Name = "Corrections Required?")]
        public bool CorrectionsRequired { get; set; }
        public string? CorrectionsDescription { get; set; }

        public string? Remarks { get; set; }

        // UI Helpers
        public bool IsEdit { get; set; }
        public bool CanEdit { get; set; }
        public List<SelectListItem> InspectionTypeLookup { get; set; } = new();
        public List<SelectListItem> InspectionMethodLookup { get; set; } = new();
        public List<SelectListItem> InspectionResultLookup { get; set; } = new();
    }


    public class CustomsInspectionListItemVm
    {
        public Guid Id { get; set; }

        // Type info
        public string InspectionTypeName { get; set; } = string.Empty;
        public string InspectionMethodName { get; set; } = string.Empty;
        public string? InspectionResultName { get; set; }

        // Core fields
        public DateTime InspectionDate { get; set; }
        public string? InspectorName { get; set; }
        public string? Location { get; set; }

        // Status flags
        public bool DiscrepancyFound { get; set; }
        public string? DiscrepancyDescription { get; set; }
        public bool CorrectionsRequired { get; set; }
        public string? CorrectionsDescription { get; set; }

        public bool CanEdit { get; set; }
    }

    public class CustomsInspectionsTabVm
    {
        public Guid CustomsClearanceId { get; set; }
        public bool CanEdit { get; set; }
        public List<CustomsInspectionListItemVm> Inspections { get; set; } = new();
    }




}
