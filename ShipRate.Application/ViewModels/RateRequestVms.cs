using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{

    public class SellQuoteEmailVm
    {

        public int CompanyId { get; set; }
        public string CompanyLogoContentId => $"company-logo-{CompanyId}";
        public bool IsPreview { get; set; } = true;
        public string CompanyLogoSource => IsPreview ? $"/logos/logo-{CompanyId}.png" : $"cid:{CompanyLogoContentId}";


        public bool canSendEmail { get; set; } = false;
        public string QuoteNo { get; set; }
        public DateTime QuoteDate { get; set; }
        public DateTime? ResponseDueDate { get; set; }

        public string CustomerName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public string TransportType { get; set; }
        public string ServiceType { get; set; }
        public string CargoType { get; set; }
        public string Incoterm { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public decimal? Volume { get; set; }

        public int? PackageCount { get; set; }
        public string PackageSummary { get; set; }

        public DateTime? CargoReadyDate { get; set; }
        public string Commodity { get; set; }

        public string Remarks { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyWebsite { get; set; }
        public List<SellQuoteContainerVm> Containers { get; set; } = new();
        public List<CargoDetailDetailsVm> CargoDetails { get; set; } = new();
        public List<SellQuoteItemVm> SellQuoteItems { get; set; } = new();
    }
    public class SellQuoteContainerVm
    {
        public string SizeTypeName { get; set; }
        public int Counts { get; set; }
    }
    public class SellQuoteItemVm
    {
        public int LineNumber { get; set; }
        public string ChargeTypeName { get; set; }
        public string SizeTypeName { get; set; }
        public decimal Counts { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class RateInquiryEmailVm
    {
        public int CompanyId { get; set; }
        public string CompanyLogoContentId => $"company-logo-{CompanyId}";
        public bool IsPreview { get; set; } = true;
        public string CompanyLogoSource => IsPreview ? $"/logos/logo-{CompanyId}.png" : $"cid:{CompanyLogoContentId}";

        public bool canSendEmail { get; set; } = false;
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
        public DateTime? ResponseDueDate { get; set; }

        public string VendorName { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }

        public string TransportType { get; set; }
        public string ServiceType { get; set; }
        public string CargoType { get; set; }
        public string Incoterm { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public decimal? Volume { get; set; }

        public int? PackageCount { get; set; }
        public string PackageSummary { get; set; }

        public DateTime? CargoReadyDate { get; set; }
        public string Commodity { get; set; }

        public string Remarks { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyWebsite { get; set; }
        public List<RateInquiryContainerVm> Containers { get; set; } = new();
        public List<CargoDetailDetailsVm> CargoDetails { get; set; } = new();
    }
    public class RateInquiryPackageVm
    {
        public int RowNo { get; set; }

        public string PackageType { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public string DimensionUnit { get; set; } = "cm";

        public decimal? Weight { get; set; }
        public string WeightUnit { get; set; } = "kg";
    }
    public class RateInquiryContainerVm
    {
        public string SizeTypeName { get; set; }
        public int Counts { get; set; }
    }



    public class SubmitQuoteVm
    {
        public Guid Id { get; set; }

        [Required]
        public Guid RateRequestInquiryId { get; set; }

        public Guid? CarrierId { get; set; }
        public Guid? VendorId { get; set; }

        public string? CarrierName { get; set; }
        public string? VendorName { get; set; }




        [Required]
        [MaxLength(40)]
        public string QuotationNo { get; set; } = string.Empty;

        [Required]
        public DateTime QuotationDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? OptionsServiceName { get; set; }

        [MaxLength(50)]
        public string? VehicleName { get; set; }

        [MaxLength(30)]
        public string? TransportMovementNo { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public int? TransitDays { get; set; }

        public bool IsDirect { get; set; }

        public DateTime? ValidUntil { get; set; }

        [MaxLength(4000)]
        public string? Remarks { get; set; }

        public List<SubmitQuoteItemVm> Items { get; set; } = new();


        public List<SelectListItem> CarrierOptions { get; set; } = new();
        public List<SelectListItem> VendorOptions { get; set; } = new();
        public List<SelectListItem> ChargeTypeOptions { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypeOptions { get; set; } = new();
        public List<SelectListItem> CurrencyOptions { get; set; } = new();



    }

    public class SubmitQuoteItemVm
    {
        [Required]
        [Display(Name = "Charge Type")]
        public int? ChargeTypeId { get; set; }

        [Display(Name = "Container Size")]
        public int? ContainerSizeTypeId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal? Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit rate must be greater than zero.")]
        public decimal? UnitRate { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public decimal TotalAmount => (Quantity ?? 0) * (UnitRate ?? 0);
    }

    public class RejectInquiryVm
    {
        [Required]
        public Guid RateRequestInquiryId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
    }
    public class RejectSellQuoteVm
    {
        [Required]
        public Guid SellQuoteId { get; set; }
        [Required]
        [MaxLength(4000)]
        public string Comments { get; set; } = string.Empty;
    }

    public class RejectRateRequestVm
    {
        [Required]
        public Guid RateRequestId { get; set; }
        [Required]
        [MaxLength(4000)]
        public string Comments { get; set; } = string.Empty;
    }





    public class AcceptSellQuoteVm
    {
        [Required]
        public Guid SellQuoteId { get; set; }
        [Required]
        [MaxLength(4000)]
        public string Comments { get; set; } = string.Empty;
    }



    public class VendorInquiryDashboardVm
    {
        public VendorInquirySearchVm Search { get; set; } = new();


        public List<VendorInquiryRowVm> Inquiries { get; set; } = new();

        public Guid? SelectedInquiryId { get; set; }

        public List<VendorQuoteResponseRowVm> Responses { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

    public class VendorInquirySearchVm
    {
        public int? StatusId { get; set; }
        public string? InquiryNo { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class VendorInquiryRowVm
    {
        public Guid Id { get; set; }

        public string InquiryNo { get; set; } = "";
        public DateTime InquiryDate { get; set; }

        public string Origin { get; set; } = "";
        public string Destination { get; set; } = "";

        public string CargoType { get; set; } = "";
        public decimal? Weight { get; set; }

        public string Status { get; set; } = "";

        public int ResponseCount { get; set; }
    }
    public class VendorQuoteResponseRowVm
    {
        public Guid Id { get; set; }

        public string QuoteNo { get; set; } = "";
        public DateTime QuoteDate { get; set; }

        public string ServiceName { get; set; } = "";
        public string TransitTime { get; set; } = "";

        public string CarrierName { get; set; } = "";
        public string VendorName { get; set; } = "";
        public string TransportBy { get; set; } = "";

        public string CreatedByUserName { get; set; } = "";
        public string LastEditedByUserName { get; set; } = "";
        public string LastActionDateTime { get; set; } = "";

        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "";
        public string Status { get; set; } = "";
        public int StatusId { get; set; }
    }

    public class QuoteItemRowVm
    {
        public SubmitQuoteVm Parent { get; set; } = default!;
        public int Index { get; set; }
    }


    public class RRDetailsVM
    {
        public string RateRequestNumber { get; set; }
        public DateTime RequestDate { get; set; }


        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Incoterm { get; set; }
        public DateTime? CargoReadyDate { get; set; }
        public string Commodity { get; set; }
        public string Remarks { get; set; }
        public string CompanyName { get; set; }

        public List<RRContainerVM> Containers { get; set; } = new();
        public List<CargoDetailDetailsVm> CargoDetails { get; set; } = new();
        public List<RRTransportTypeVM> TransportTypes { get; set; } = new();
    }
    public class RRContainerVM
    {
        public int Counts { get; set; }
        public string SizeTypeName { get; set; }
    }
    public class RRTransportTypeVM
    {
        public int Sequence { get; set; }
        public string TransportTypeName { get; set; }
    }

}
