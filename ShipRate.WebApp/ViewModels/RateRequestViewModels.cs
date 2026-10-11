using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.WebApp.ViewModels
{
    // ViewModels/RateRequest/RateRequestListViewModel.cs
    public class RateRequestListViewModel
    {
        public List<RateRequestItemViewModel> Items { get; set; } = new();
        public RateRequestSearchViewModel Search { get; set; } = new();
        public PaginationViewModel Pagination { get; set; } = new();
    }

    // ViewModels/RateRequest/RateRequestSearchViewModel.cs
    public class RateRequestSearchViewModel
    {
        public string? CustomerName { get; set; }
        public string? ContactName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailAddress { get; set; }
        public int? StatusId { get; set; }
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }

        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }

        public DateTime? CargoReadinessDateFrom { get; set; }
        public DateTime? CargoReadinessDateTo { get; set; }


        // Service requirement filters
        public bool? CustomsClearanceRequired { get; set; }
        public bool? InsuranceRequired { get; set; }
        public bool? WarehousingRequired { get; set; }
        public bool? StuffingRequired { get; set; }
        public bool? ExternalTransitRequired { get; set; }
        public bool? InternalTransitRequired { get; set; }
        public bool? PickupRequired { get; set; }
        public bool? DeliveryRequired { get; set; }


        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ViewModels/RateRequest/RateRequestItemViewModel.cs
    public class RateRequestItemViewModel
    {
        public Guid Id { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }
        public DateTime CargoReadinessDate { get; set; }
        public string? StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? LastEditedByUserName { get; set; }
        public string? SalesUserName { get; set; }
    }

    // ViewModels/RateRequest/RateRequestFormViewModel.cs
    public class RateRequestFormViewModel
    {
        public Guid? Id { get; set; }

        //Req Info
        public int? CompanyId { get; set; }
        public string? RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public int SourceId { get; set; }


        // Customer Info
        public Guid? CustomerId { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerContactId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string? Introducer { get; set; }

        // Cargo Details
        public int CargoHandlingTypeId { get; set; }
        public int CargoNatureId { get; set; }
        public Guid? CargoCommodityId { get; set; }
        public string CargoCommodityDescription { get; set; } = string.Empty;
        public bool IsHazardous { get; set; }
        public Guid? DGId { get; set; }
        public string? DGDescription { get; set; }
        public string OriginHSCode { get; set; } = string.Empty;
        public string DestinationHSCode { get; set; } = string.Empty;
        public int LoadTypeId { get; set; }

        // Shipment Details
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }
        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }

        public int? TradeTypeId { get; set; }
        public int? MovementTypeId { get; set; }
        public int? PaymentTypeId { get; set; }
        public DateTime CargoReadinessDate { get; set; }
        public int? QuotationPriorityId { get; set; }
        public bool HasEx1 { get; set; }
        public bool? TransitHasTranshipment { get; set; }
        public int? SellToCountryId { get; set; }

        // Pickup & Delivery
        public bool PickupRequired { get; set; }
        public string? PickupAddress { get; set; }
        public string? OriginZipCode { get; set; }
        public bool DeliveryRequired { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DestinationZipCode { get; set; }


        // Services Required
        public bool ExternalTransitRequired { get; set; }
        public bool InternalTransitRequired { get; set; }

        public bool CustomsClearanceRequired { get; set; }
        public bool InsuranceRequired { get; set; }
        public bool WarehousingRequired { get; set; }
        public bool StuffingRequired { get; set; }

        // Financial
        public long? CargoValue { get; set; }
        public int? CurrencyId { get; set; }

        // Assignment
        public Guid? SalesManagerId { get; set; }
        public Guid? SalesmanId { get; set; }
        public int? StatusId { get; set; }

        public string Remarks { get; set; } = string.Empty;

        // Child Collections
        public List<RateRequestTransportTypeViewModel> TransportTypes { get; set; } = new();
        public List<RateRequestPackageViewModel> Packages { get; set; } = new();
        public List<RateRequestContainerViewModel> Containers { get; set; } = new();



        // SelectLists for dropdowns

        public SelectList? Sources { get; set; }
        public SelectList? Companies { get; set; }
        public SelectList? Customers { get; set; }
        public SelectList? Origins { get; set; }
        public SelectList? Destinations { get; set; }
        public SelectList? CargoHandlingTypes { get; set; }
        public SelectList? CargoNatures { get; set; }
        public SelectList? LoadTypes { get; set; }
        public SelectList? CargoCommodities { get; set; }
        public SelectList? Statuses { get; set; }
        public SelectList? TradeTypes { get; set; }
        public SelectList? PaymentTypes { get; set; }
        public SelectList? MovementTypes { get; set; }
        public SelectList? Currencies { get; set; }

        public List<ContainerSizeType>? ContainerTypesSelectList { get; set; } = new();
        public List<TransportType>? TransportTypesSelectList { get; set; }
        public List<PackageType>? PackageTypesSelectList { get; set; }


    }

    // ViewModels/RateRequest/RateRequestTransportTypeViewModel.cs
    public class RateRequestTransportTypeViewModel
    {
        public Guid? Id { get; set; }
        public int Sequence { get; set; }
        public int TransportTypeId { get; set; }
    }

    public class RateRequestPackageViewModel
    {
        public Guid? Id { get; set; }
        public int PackageTypeId { get; set; }

        public decimal? UnitLength { get; set; }
        public decimal? UnitWidth { get; set; }
        public decimal? UnitHeight { get; set; }
        public int? LengthMeasurementUnitId { get; set; }

        public decimal? UnitVolume { get; set; }
        public int? VolumeMeasurementUnitId { get; set; }

        public decimal? UnitGrossWeight { get; set; }
        public decimal? UnitNetWeight { get; set; }
        public int? WeightMeasurementUnitId { get; set; }

        public int UnitCount { get; set; }

        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }
        public decimal? TotalVolume { get; set; }

        public decimal? TotalLength { get; set; }
        public decimal? TotalWidth { get; set; }
        public decimal? TotalHeight { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }

    public class RateRequestContainerViewModel
    {
        public Guid? Id { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public decimal Counts { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    public class RateRequestInquiryViewModel
    {
        public Guid RateRequestId { get; set; }

        public string RateRequestNumber { get; set; }

        public List<SelectListItem> Carriers { get; set; }

        public List<SelectListItem> Agents { get; set; }

        public List<RateRequestInquiryRowVM> Existing { get; set; }


        public List<InquiryDto> Inquiries { get; set; }

    }
    public class InquiryDto
    {
        public Guid Id { get; set; }
        public string CarrierName { get; set; }
        public string VendorEmailAddress { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string RequestNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public string? SentByUserName { get; set; }
        public DateTime? SentDate { get; set; }
    }
    public class RateRequestInquiryRowVM
    {
        public Guid PartyId { get; set; }
    }

    public class SaveRateRequestInquiryVM
    {
        public Guid RateRequestId { get; set; }
        public List<RateRequestInquiryRowVM> Items { get; set; }
    }

    public class RateRequestInquiryVM
    {
        public Guid RateRequestId { get; set; }

        // اطلاعات عمومی
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }
        public int? TransportTypeId { get; set; }
        public string? Remarks { get; set; }

        // لیست‌های مورد نیاز برای SelectList
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();

        // لیست فروشندگان برای نمایش در UI
        public List<CarrierItemVM> AvailableCarriers { get; set; } = new();
        public List<Guid> SelectedCarrierIds { get; set; } = new();
    }

    public class CarrierVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CarrierItemVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }


    public class SaveInquiryVM
    {
        public Guid RateRequestId { get; set; }
        public Guid[] SelectedCarrierIds { get; set; } = Array.Empty<Guid>();
    }
    public class RateRequestInquiryListViewModel
    {
        public Guid RateRequestId { get; set; }
        public string RequestNo { get; set; }
        public List<InquiryDto> Inquiries { get; set; } = new();
    }

    public class InquiryEmailViewModel
    {
        public Guid RateRequestId { get; set; }
        public Guid InquiryId { get; set; }
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
    }

    public class BuyQuoteListViewModel
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }
        public Guid InquiryId { get; set; }

        public string? InqNo { get; set; }
        public DateTime? InqDate { get; set; }

        public string? QuotationNo { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? CarrierName { get; set; }
        public string? VendorName { get; set; }
        public string? OptionsServiceName { get; set; }
        public string? VehicleName { get; set; }
        public string? TransportMovementNo { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public int? TransitDays { get; set; }
        public bool IsDirect { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? Remarks { get; set; }
        public string CreateUserName { get; set; } = "";
        public string UpdateUserName { get; set; } = "";
        public List<BuyQuoteItemListViewModel> Items { get; set; } = new();
    }
    public class BuyQuoteItemListViewModel
    {
        public Guid Id { get; set; }
        public Guid BuyQuoteId { get; set; }
        public string? ChargeTypeCode { get; set; }
        public string? ChargeTypeName { get; set; }
        public string? SizeTypeShortName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string? Remarks { get; set; }


    }
    public class CreateSellQuoteViewModel
    {
        public Guid Id { get; set; }
        public Guid BuyQuoteId { get; set; }
        public Guid RateRequestId { get; set; }

        public Guid? CarrierId { get; set; }

        public string? QuotationNo { get; set; }
        public string? ServiceName { get; set; }
        public string? VehicleNames { get; set; }
        public string? MovementNumbers { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ValidUntil { get; set; }

        // لیستی از آیتم‌ها شامل اطلاعات خرید برای مقایسه
        public List<SellQuoteItemEditModel> Items { get; set; } = new();
        public List<SelectListItem> Carriers { get; set; } = new();
    }

    public class SellQuoteItemEditModel
    {
        public int ChargeTypeId { get; set; }
        public string? ChargeTypeName { get; set; }
        public int? ContainerSizeTypeId { get; set; }
        public string? SizeTypeName { get; set; }

        // مقادیر خرید (فقط برای نمایش)
        public decimal? BuyUnitRate { get; set; }
        public decimal? BuyTotal { get; set; }
        public string? CurrencyCode { get; set; }

        // مقادیر فروش (قابل ویرایش)
        public decimal? SellUnitRate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? SellTotal { get; set; }
        public int? CurrencyId { get; set; }
    }

    public class SellQuoteListIndex
    {
        public Guid RateRequestId { get; set; }
        public List<SellQuoteListViewModel> SellQuotesList { get; set; } = new List<SellQuoteListViewModel>();
    }



    public class SellQuoteListViewModel
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }

        public string? QuotationNo { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string? OptionsServiceName { get; set; }
        public string? VehicleName { get; set; }
        public string? TransportMovementNo { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public int? TransitDays { get; set; }
        public bool IsDirect { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? Remarks { get; set; }
        public string CreateUserName { get; set; } = "";
        public string UpdateUserName { get; set; } = "";
        public string Status { get; set; } = "";
        public int StatusId { get; set; }
        public List<SellQuoteItemListViewModel> Items { get; set; } = new();
    }
    public class SellQuoteItemListViewModel
    {
        public Guid Id { get; set; }
        public Guid SellQuoteId { get; set; }
        public string? ChargeTypeCode { get; set; }
        public string? ChargeTypeName { get; set; }
        public string? SizeTypeShortName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string? Remarks { get; set; }
    }
}