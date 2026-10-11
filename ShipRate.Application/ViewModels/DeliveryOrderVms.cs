using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public sealed class DeliveryOrderSearchFilter
    {
        public string? SearchTerm { get; set; } // برای جستجوی DONo یا BLNo
        public bool? IsFinalized { get; set; }
        public int? StatusId { get; set; }
        public int? ClearanceTypeId { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? BLId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> ClearanceTypes { get; set; } = new();
        public List<SelectListItem> Carriers { get; set; } = new();

    }
    public sealed class DeliveryOrderListItemVm
    {
        public Guid DeliveryOrderId { get; set; }
        public Guid BLId { get; set; }
        public string? DONo { get; set; }
        public DateTime? DoDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BLNo { get; set; }
        public string? ConsigneeName { get; set; }
        public string? ClearanceTypeName { get; set; }
        public int ClearanceTypeId { get; set; }
        public bool IsFinalized { get; set; }
        public DateTime? FinalizedAt { get; set; }
        public int ContainerCount { get; set; }
        public string? CarrierName { get; set; }
        public string? StatusName { get; set; }
        public int? StatusId { get; set; }
        public int DOOperationModeId { get; set; }
        public int Serial { get; set; }
        public string DOOperationModeName { get; set; }
        public string DOOperationModeLocalName { get; set; }

        public string? CreateUserName { get; set; }
        public string? UpdateUserName { get; set; }
        public DateTime? CreatedAT { get; set; }
        public DateTime? UpdateAT { get; set; }



    }

    public sealed class DeliveryOrderListWrapperVm
    {
        public List<DeliveryOrderListItemVm> Items { get; set; } = new();
        public DeliveryOrderSearchFilter Filter { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / Filter.PageSize);
    }





    public sealed class DeliveryOrderDetailsVm
    {
        public Guid DeliveryOrderId { get; set; }
        public string? DONo { get; set; }
        public DateTime? DoDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string? ClearanceTypeName { get; set; }
        public string? GoodsDescription { get; set; }
        public string? BLReleaseTypeName { get; set; }
        public int? OriginalBLCount { get; set; }
        public string? Remarks { get; set; }

        public bool IsFinalized { get; set; }
        public DateTime? FinalizedAt { get; set; }

        public DeliveryOrderBLSummaryVm? BLSummary { get; set; }
        public List<DeliveryOrderContainerVm> Containers { get; set; } = new();
        public List<DeliveryOrderCargoVm> CargoItems { get; set; } = new();
    }



    public sealed class FinalizeDeliveryOrderRequestVm
    {
        public Guid DeliveryOrderId { get; set; }
    }
    public sealed class FinalizeDeliveryOrderResultVm
    {
        public bool Success { get; set; }
        public Guid DeliveryOrderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StatusName { get; set; }
    }

    public sealed class DeliveryOrderCargoVm
    {
        public Guid Id { get; set; }
        public Guid DeliveryOrderId { get; set; }
        public Guid? BLCargoId { get; set; }

        public string? PackageTypeName { get; set; }
        public decimal? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string? Measurement { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? DescriptionOfGoods { get; set; }
    }
    public class DeliveryOrderContainerVm
    {
        public Guid DeliveryOrderContainerId { get; set; }
        public Guid BLContainerId { get; set; }
        public string ContainerNo { get; set; } = string.Empty;
        public string SealNo { get; set; } = string.Empty;
        public string? ContainerType { get; set; }
        public string? ContainerSize { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public int? PackageCount { get; set; }
        public int? TareWeight { get; set; }
        public string? MovementType { get; set; }
    }

    public class CreateDeliveryOrderPageVm
    {
        public Guid? DeliveryOrderId { get; set; }

        [Required]
        public Guid? BLId { get; set; }

        public string BLNo { get; set; } = string.Empty;
        public string MasterBLNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "DO No")]
        public string DONo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "DO Date")]
        public DateTime DoDate { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime? ArrivalDate { get; set; }

        [Required]
        [Display(Name = "Clearance Type")]
        public int? ClearanceTypeId { get; set; }

        [Required]
        [Display(Name = "Operational Mode")]
        public int? DOOperationalModeId { get; set; }

        [Required]
        [Display(Name = "BL Release Type")]
        public int? BLReleaseTypeId { get; set; }

        [Display(Name = "Original BL Count")]
        public int OriginalBLCount { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public string StatusText { get; set; } = "Draft";
        public int? StatusId { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? CarrierAgentId { get; set; }
        public Guid? ExternalIssuerId { get; set; }
        public string? ExternalIssuerText { get; set; }
        public DateTime? ReceivedAtDateTime { get; set; }
        public Guid? ReceivedByUserId { get; set; }
        public int? ReceiveMethodId { get; set; }


        public Guid? TIPId { get; set; }



        public List<SelectListItem> ClearanceTypes { get; set; } = new();
        public List<SelectListItem> BLReleaseTypes { get; set; } = new();
        public List<SelectListItem> DOOperationalModes { get; set; } = new();

        public List<SelectListItem> Carriers { get; set; } = new();
        public List<SelectListItem> Agents { get; set; } = new();
        public List<SelectListItem> ExternalIssuers { get; set; } = new();
        public List<SelectListItem> ReceiveMethods { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> TIPs { get; set; } = new();
    }


    public class SaveDeliveryOrderDraftRequestVm
    {

        public int CompanyId { get; set; }
        public int DOOperationalModeId { get; set; }

        public Guid? DeliveryOrderId { get; set; }

        [Required]
        public Guid? BLId { get; set; }

        [Required]
        public string DONo { get; set; } = string.Empty;

        [Required]
        public DateTime DoDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        [Required]
        public int? ClearanceTypeId { get; set; }

        [Required]
        public int? BLReleaseTypeId { get; set; }

        public int OriginalBLCount { get; set; }
        public string? Remarks { get; set; }


        public Guid? CarrierId { get; set; }
        public Guid? CarrierAgentId { get; set; }
        public Guid? ExternalIssuerId { get; set; }
        public string? ExternalIssuerText { get; set; }
        public DateTime? ReceivedAtDateTime { get; set; }
        public Guid? ReceivedByUserId { get; set; }
        public int? ReceiveMethodId { get; set; }

        public Guid? TIPId { get; set; }



    }

    public class SaveDeliveryOrderDraftResultVm
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? DeliveryOrderId { get; set; }
    }


    public class DeliveryOrderBLLookupItemVm
    {
        public Guid BLId { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public string? BookingNo { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string ConsigneeName { get; set; } = string.Empty;
    }

    public class DeliveryOrderBLSummaryVm
    {
        public Guid BLId { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public string? BookingNo { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string? VesselName { get; set; }
        public string? VoyageNumber { get; set; }
        public string? POL { get; set; }
        public string? POD { get; set; }
        public DateTime? IssueDate { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public int? PackageCount { get; set; }
        public int ContainerCount { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public string? ConsigneeText { get; set; }
        public string? NotifyText { get; set; }


        public List<DeliveryOrderContainerVm> Containers { get; set; } = new();
        public List<DeliveryOrderCargoVm> Cargos { get; set; } = new();

    }

    public sealed class CancelDeliveryOrderVm
    {
        [Required]
        public Guid DeliveryOrderId { get; set; }

        public string? DONo { get; set; }
        public string? BLNo { get; set; }

        [Required(ErrorMessage = "Please provide a reason for cancellation.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        [Display(Name = "Cancellation Reason")]
        public string CancellationReason { get; set; } = string.Empty;
    }
    public sealed class DeliveryOrderDocumentPanelVm
    {
        public Guid DeliveryOrderId { get; set; }
        public List<DeliveryOrderDocumentCardVm> Cards { get; set; } = new();
    }
    public sealed class DeliveryOrderDocumentCardVm
    {
        public Guid DeliveryOrderId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public Guid? DocumentId { get; set; }
        public List<DeliveryOrderDocumentAttachmentVm> Attachments { get; set; } = new();
    }

    public sealed class DeliveryOrderDocumentAttachmentVm
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FileSizeText { get; set; } = string.Empty;
    }


    public class DocumentAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[]? Content { get; set; }
    }

    public class ChargeRowVm
    {
        public Guid ChargeId { get; set; }
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeDescription { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty; // Inbound/Outbound
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal? TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }

    public class InvoiceRowVm
    {
        public Guid InvoiceId { get; set; }
        public int InvoiceModelId { get; set; }
        public int DirectionId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty; // Invoice/Credit Note/...
        public string SubjectName { get; set; } = string.Empty; // Invoice/Credit Note/...
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public bool IsPosted { get; set; }
        public string FNFName { get; set; }
        public string InvocieModelName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string DirectionName { get; set; }
        public decimal SettledAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public int Serial { get; set; }
        public string SerialPrefix { get; set; } = string.Empty;
        public string SerialDisplay => Serial <= 0
            ? "-"
            : (string.IsNullOrWhiteSpace(SerialPrefix) ? Serial.ToString() : $"{SerialPrefix} {Serial}");



        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreateUserName { get; set; }
        public string? UpdateUserName { get; set; }

    }

    public class BLInvoicesVm
    {
        public Guid JobId { get; set; }
        public Guid BLId { get; set; }
        public List<InvoiceRowVm> Invoices { get; set; } = new();
        public string? FilterType { get; set; }
    }

    public class FreightLetterUpsertVm
    {
        public Guid? Id { get; set; }
        public Guid DOId { get; set; }

        public Guid? BLId { get; set; }
        public string? MBLNo { get; set; }
        public string? HBLNo { get; set; }

        public Guid CustomsOfficeId { get; set; }
        public decimal FreightValue { get; set; }
        public int CurrencyId { get; set; }

        public string? FromLocationName { get; set; }
        public string? ToLocationName { get; set; }
        public string? CustomerName { get; set; }

        public int? LetterNo { get; set; }
        public DateTime LetterDate { get; set; } = DateTime.Today;

        public bool IsSaved { get; set; }

        // Dropdowns
        public List<SelectListItem> CustomsOffices { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
    }


}
