using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    
        public class RateRequestViewDto
        {

            //Customer Information
            public Guid CustomerId { get; set; }
            public Guid LeadId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string ContactName { get; set; } = string.Empty;
            public string MobileNo { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;


            [Required]
            public Guid OriginId { get; set; }

            [Required]
            public Guid DestinationId { get; set; }

            [Required]
            public int TransPortTypeId { get; set; }



            [Required]
            public int CargoHandlingTypeId { get; set; }
            [Required]
            public int CargoNatureId { get; set; }
            [Required]
            public Guid CargoCommodityId { get; set; }
            public string CargoCommodityDescription { get; set; } = string.Empty;
            public bool IsHazardous { get; set; }
            public string OriginHSCode { get; set; } = string.Empty;
            public string DestinationHSCode { get; set; } = string.Empty;
            public int LoadTypeId { get; set; }
            public int TradeTypeId { get; set; }

            public int PackageTypeId { get; set; }
            public decimal PackageCounts { get; set; }
            public DateTime CargoReadinessDate { get; set; }


        }
        public class RateRequestContainerViewOldDto
        {
            public int ContainerSizeTypeId { get; set; }
            public string ShortName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public decimal Counts { get; set; }
            public string Remarks { get; set; } = string.Empty;
        }
        public class RateRequestListDto
        {
            public List<RateRequestItemDto> Items { get; set; } = new();
            public RateRequestSearchDto Search { get; set; } = new();
            public PaginationDto Pagination { get; set; } = new();
        }

        // ViewModels/RateRequest/RateRequestSearchViewModel.cs
        public class RateRequestSearchDto
        {
            public string? CustomerName { get; set; }
            public string? ContactName { get; set; }
            public string? MobileNo { get; set; }
            public string? EmailAddress { get; set; }
            public int? StatusId { get; set; }
            public Guid? OriginId { get; set; }
            public Guid? DestinationId { get; set; }
            public DateTime? CargoReadinessDateFrom { get; set; }
            public DateTime? CargoReadinessDateTo { get; set; }
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
        public class AssignSalesUserDto
        {
            public Guid CustomerId { get; set; }
            public string PersonName { get; set; } = "";
            public Guid? CurrentSalesUserId { get; set; }
            public string? CurrentSalesUserName { get; set; }
            public Guid? SelectedSalesUserId { get; set; }

            public List<SalesUserDropdownItemDto> SalesUsers { get; set; } = new();
        }

        public class SalesUserDropdownItemDto
        {
            public Guid Id { get; set; }
            public string FullName { get; set; } = "";
        }

        // ViewModels/RateRequest/RateRequestItemViewModel.cs
        public class RateRequestItemDto
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
        }

        // ViewModels/RateRequest/RateRequestFormViewModel.cs
        public class RateRequestFormDto
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
            public List<RateRequestTransportTypeDto> TransportTypes { get; set; } = new();
            public List<RateRequestPackageDto> Packages { get; set; } = new();
            public List<RateRequestContainerDto> Containers { get; set; } = new();



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

            public List<ContainerSizeType>? ContainerTypesSelectList { get; set; } = new();
            public List<TransportType>? TransportTypesSelectList { get; set; }
            public List<PackageType>? PackageTypesSelectList { get; set; }
        }


        // ViewModels/RateRequest/RateRequestTransportTypeViewModel.cs
        public class RateRequestTransportTypeDto
        {
            public Guid? Id { get; set; }
            public int Sequence { get; set; }
            public int TransportTypeId { get; set; }
        }

        public class RateRequestPackageDto
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

        public class RateRequestContainerDto
        {
            public Guid? Id { get; set; }
            public int ContainerSizeTypeId { get; set; }
            public decimal Counts { get; set; }
            public string Remarks { get; set; } = string.Empty;
        }

        public class PaginationDto
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }

            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        }
        public class RateRequestInquiryDto
        {
            public Guid RateRequestId { get; set; }

            public string RateRequestNumber { get; set; }

            public List<SelectListItem> Carriers { get; set; }

            public List<SelectListItem> Agents { get; set; }

            public List<RateRequestInquiryRowDto> Existing { get; set; }


            public List<InquiryDto> Inquiries { get; set; }

        }
        public class InquiryDto
        {
            public Guid Id { get; set; }
            public string CarrierName { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }
            public string RequestNo { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }

            public string? SentByUserName { get; set; }
            public DateTime? SentDate { get; set; }
        }
        public class RateRequestInquiryRowDto
        {
            public Guid PartyId { get; set; }
        }

        public class SaveRateRequestInquiryDto
        {
            public Guid RateRequestId { get; set; }
            public List<RateRequestInquiryRowDto> Items { get; set; }
        }

        public class RateRequestInquiryVMDto
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
            public List<CarrierItemDto> AvailableCarriers { get; set; } = new();
            public List<Guid> SelectedCarrierIds { get; set; } = new();
        }

        public class CarrierDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class CarrierItemDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }


        public class SaveInquiryDto
        {
            public Guid RateRequestId { get; set; }
            public Guid[] SelectedCarrierIds { get; set; } = Array.Empty<Guid>();
        }
        public class RateRequestInquiryListDto
        {
            public Guid RateRequestId { get; set; }
            public string RequestNo { get; set; }
            public List<InquiryDto> Inquiries { get; set; } = new();
        }

        public class InquiryEmailDto
        {
            public Guid RateRequestId { get; set; }
            public Guid InquiryId { get; set; }
            public string InquiryNo { get; set; }
            public DateTime InquiryDate { get; set; }
        }

        public class BuyQuoteListDto
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
            public List<BuyQuoteItemListDto> Items { get; set; } = new();
        }
        public class BuyQuoteItemListDto
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

        public class CreateSellQuoteDto
        {
            public Guid Id { get; set; }
            public Guid BuyQuoteId { get; set; }
            public Guid RateRequestId { get; set; }

            public string? QuotationNo { get; set; }
            public string? ServiceName { get; set; }
            public string? VehicleNames { get; set; }
            public string? MovementNumbers { get; set; }
            public string? Remarks { get; set; }
            public DateTime? ETD { get; set; }
            public DateTime? ETA { get; set; }
            public DateTime? ValidUntil { get; set; }

            // لیستی از آیتم‌ها شامل اطلاعات خرید برای مقایسه
            public List<SellQuoteItemEditDto> Items { get; set; } = new();
        }

        public class SellQuoteItemEditDto
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

        public class SellQuoteListIndexDto
        {
            public Guid RateRequestId { get; set; }
            public List<SellQuoteListDto> SellQuotesList { get; set; } = new List<SellQuoteListDto>();
        }



        public class SellQuoteListDto
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
            public List<SellQuoteItemListDto> Items { get; set; } = new();
        }
        public class SellQuoteItemListDto
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

    //public class RateRequestDto
    //{
    //    public Guid Id { get; set; }
    //    public string CustomerName { get; set; }
    //    public string ContactName { get; set; }
    //    public string MobileNo { get; set; }
    //    public string EmailAddress { get; set; }
    //    public DateTime CargoReadinessDate { get; set; }
    //    public string StatusName { get; set; }
    //    public string OriginName { get; set; }
    //    public string DestinationName { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //}



    //public class RateRequestSearchDto
    //{
    //    [StringLength(50)]
    //    public string RateRequestNumber { get; set; }

    //    public int? CustomerId { get; set; }
    //    public int? OriginLocationId { get; set; }
    //    public int? DestinationLocationId { get; set; }
    //    public int? StatusId { get; set; }
    //    public int? CargoHandlingTypeId { get; set; }
    //    public int? TransportTypeId { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? RequestDateFrom { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? RequestDateTo { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? ReadyDateFrom { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? ReadyDateTo { get; set; }

    //    [StringLength(500)]
    //    public string ReferenceNumber { get; set; }

    //    [Range(1, int.MaxValue)]
    //    public int PageNumber { get; set; } = 1;

    //    [Range(10, 100)]
    //    public int PageSize { get; set; } = 20;
    //}


    //public class RateRequestCreateUpdateDto
    //{
    //    // Customer Information
    //    [Required(ErrorMessage = "Customer is required")]
    //    public int CustomerId { get; set; }

    //    // Cargo Information
    //    [Required(ErrorMessage = "Cargo handling type is required")]
    //    public int CargoHandlingTypeId { get; set; }

    //    public int? CargoTypeId { get; set; }

    //    [StringLength(500)]
    //    public string CargoDescription { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Total weight must be greater than 0")]
    //    public decimal? TotalWeight { get; set; }

    //    public int? WeightUnitId { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Total volume must be greater than 0")]
    //    public decimal? TotalVolume { get; set; }

    //    public int? VolumeUnitId { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Total value must be greater than 0")]
    //    public decimal? TotalValue { get; set; }

    //    public int? CurrencyId { get; set; }

    //    public bool IsHazardous { get; set; }
    //    public bool IsPerishable { get; set; }
    //    public bool IsFragile { get; set; }
    //    public bool RequiresTemperatureControl { get; set; }

    //    [Range(-50, 50)]
    //    public decimal? TemperatureMin { get; set; }

    //    [Range(-50, 50)]
    //    public decimal? TemperatureMax { get; set; }

    //    // Route Information
    //    [Required(ErrorMessage = "Origin location is required")]
    //    public int OriginLocationId { get; set; }

    //    [Required(ErrorMessage = "Destination location is required")]
    //    public int DestinationLocationId { get; set; }

    //    public int? IncotermsId { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? ReadyDate { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? RequiredDeliveryDate { get; set; }

    //    // Request Information
    //    [Required(ErrorMessage = "Status is required")]
    //    public int StatusId { get; set; }

    //    [DataType(DataType.Date)]
    //    public DateTime? ValidUntil { get; set; }

    //    [StringLength(1000)]
    //    public string SpecialInstructions { get; set; }

    //    [StringLength(500)]
    //    public string ReferenceNumber { get; set; }

    //    // Collections
    //    public List<int> TransportTypeIds { get; set; } = new List<int>();
    //    public List<RateRequestPackageDto> Packages { get; set; } = new List<RateRequestPackageDto>();
    //    public List<RateRequestFCLDto> FCLContainers { get; set; } = new List<RateRequestFCLDto>();
    //    public RateRequestLCLLTLDto LCLLTLDetails { get; set; }
    //    public RateRequestFTLDto FTLDetails { get; set; }
    //    public List<int> DeliveryOptionIds { get; set; } = new List<int>();
    //}

    //public class RateRequestPackageDto
    //{
    //    [Required(ErrorMessage = "Package type is required")]
    //    public int PackageTypeId { get; set; }

    //    [Required(ErrorMessage = "Quantity is required")]
    //    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    //    public int Quantity { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
    //    public decimal? Length { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0")]
    //    public decimal? Width { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0")]
    //    public decimal? Height { get; set; }

    //    public int? DimensionUnitId { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    //    public decimal? Weight { get; set; }

    //    public int? WeightUnitId { get; set; }

    //    public bool IsStackable { get; set; }
    //}

    //public class RateRequestFCLDto
    //{
    //    [Required(ErrorMessage = "Container type is required")]
    //    public int ContainerTypeId { get; set; }

    //    [Required(ErrorMessage = "Quantity is required")]
    //    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    //    public int Quantity { get; set; }
    //}

    //public class RateRequestLCLLTLDto
    //{
    //    [Range(0.01, double.MaxValue, ErrorMessage = "Volume must be greater than 0")]
    //    public decimal? Volume { get; set; }

    //    public int? VolumeUnitId { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    //    public decimal? Weight { get; set; }

    //    public int? WeightUnitId { get; set; }

    //    [Range(1, int.MaxValue, ErrorMessage = "Number of pieces must be at least 1")]
    //    public int? NumberOfPieces { get; set; }
    //}

    //public class RateRequestFTLDto
    //{
    //    public int? TruckTypeId { get; set; }

    //    [Range(1, 100, ErrorMessage = "Number of trucks must be between 1 and 100")]
    //    public int? NumberOfTrucks { get; set; }

    //    [Range(0.01, double.MaxValue, ErrorMessage = "Total weight must be greater than 0")]
    //    public decimal? TotalWeight { get; set; }

    //    public int? WeightUnitId { get; set; }
    //}


    //public class LookupDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    //public class CustomerLookupDto
    //{
    //    public int CustomerId { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerCode { get; set; }
    //    public string Email { get; set; }
    //    public string Phone { get; set; }
    //}

    //public class LocationLookupDto
    //{
    //    public int LocationId { get; set; }
    //    public string LocationName { get; set; }
    //    public string City { get; set; }
    //    public string Country { get; set; }
    //    public string PortCode { get; set; }
    //}

    //public class CargoLookupDto
    //{
    //    public int CargoTypeId { get; set; }
    //    public string CargoTypeName { get; set; }
    //    public string HSCode { get; set; }
    //    public string Description { get; set; }
    //}
    //public class RateRequestDeliveryOptionDto
    //{
    //    public bool PickupRequired { get; set; }
    //    public string PickupAddress { get; set; }
    //    public bool DeliveryRequired { get; set; }
    //    public string DeliveryAddress { get; set; }
    //    public bool NeedForkLift { get; set; }
    //    public bool NeedLabour { get; set; }
    //    public string ZipCodeOrigin { get; set; }
    //    public string ZipCodeDestination { get; set; }
    //}



