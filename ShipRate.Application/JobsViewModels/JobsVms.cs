using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.JobsViewModels
{

    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    //Jobs Section
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    public class JobCompanyRoleListItemVm
    {
        public string CompanyName { get; set; }
        public string CompanyRoleTypeName { get; set; }
        public string ServiceTypeName { get; set; }
        public string ServiceTypeLocalName { get; set; }
    }

    public class JobListItemVm
    {
        public Guid Id { get; set; }

        public Guid? SalesUserId { get; set; }
        public string? SalesUserName { get; set; }

        public string? JobNumber { get; set; }
        public string? JobFileNumber { get; set; }

        public string? CompanyName { get; set; }

        public string? CustomerName { get; set; } = "-";
        public string? BookerClientName { get; set; } = "-";

        public string? TransportType { get; set; }
        public string? TradeType { get; set; }
        public int? TradeTypeId { get; set; }
        public string? LoadType { get; set; }

        //public string JobType { get; set; }
        public string? JobStatus { get; set; }

        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? MovementType { get; set; }


        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }


        public DateTime? JobDate { get; set; }
        public DateTime? CargoReadyDate { get; set; }
    }

    public class JobSearchVm
    {
        public string JobNumber { get; set; }
        public int? CompanyId { get; set; }

        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public Guid? SalesUserId { get; set; }


        public int? TradeTypeId { get; set; }

        public int? TransportTypeId { get; set; }
        public int? JobStatusId { get; set; }

        public Guid? OriginId { get; set; }
        public Guid? OriginName { get; set; }

        public Guid? DestinationId { get; set; }
        public Guid? DestinationName { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class JobIndexVm
    {
        public List<JobListItemVm> Jobs { get; set; } = new();
        public JobSearchVm Search { get; set; } = new();

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        //public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> JobStatuses { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> SalesUsers { get; set; } = new();
    }

    public class CreateJobVm
    {


        public Guid? JobId { get; set; }

        public int JobSourceTypeId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string JobNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string JobFileNumber { get; set; }

        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public Guid? BookerClientId { get; set; }
        public string? BookerClientName { get; set; }


        [Required]
        public int TransportTypeId { get; set; }

        [Required]
        public int TradeTypeId { get; set; }

        [Required]
        public int LoadTypeId { get; set; }

        [Required]
        public int CargoHandlingTypeId { get; set; }

        [Required]
        public int CargoNatureId { get; set; }

        public int? IncotermId { get; set; }

        [MaxLength(200)]
        public string? IncotermsLocationName { get; set; }

        [Required]
        public int FreightPaymentTypeId { get; set; }

        //[Required]
        //public int JobTypeId { get; set; }

        [Required]
        public int JobStatusId { get; set; }

        public DateTime? JobDate { get; set; }
        public DateTime? CargoReadyDate { get; set; }

        //public bool IsConsolMaster { get; set; }

        [Required]
        public Guid OriginLocationId { get; set; }

        public string? OriginName { get; set; }


        [Required]
        public Guid DestinationLocationId { get; set; }

        public string? DestinationName { get; set; }


        public Guid? SalesUserId { get; set; }

        [Required]
        public int MovementTypeId { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> LoadTypes { get; set; } = new();
        public List<SelectListItem> CargoHandlingTypes { get; set; } = new();
        public List<SelectListItem> CargoNatures { get; set; } = new();
        public List<SelectListItem> Incoterms { get; set; } = new();
        public List<SelectListItem> PaymentTypes { get; set; } = new();
        public List<SelectListItem> JobTypes { get; set; } = new();
        public List<SelectListItem> JobStatuses { get; set; } = new();
        public List<SelectListItem> MovementTypes { get; set; } = new();
        public List<SelectListItem> JobSourceTypes { get; set; } = new();
        public List<SelectListItem> SalesUsers { get; set; } = new();
        //Dynamin
        public List<JobContainerCreateVm> Containers { get; set; } = new();
        public List<JobPackageCreateVm> Packages { get; set; } = new();
        public List<JobCompanyRoleCreateVm> JobCompanyRoles { get; set; } = new();
        public List<JobPartyCreateVm> JobParties { get; set; } = new();
    }

    public class JobPartyCreateVm
    {
        public int Index { get; set; }

        [Required]
        public int SequenceNo { get; set; }

        [Required]
        public Guid PartyId { get; set; }

        [Required]
        public int RoleTypeId { get; set; }

        [Required]
        public bool IsPrimaryRole { get; set; }

        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public List<SelectListItem> RoleTypes { get; set; } = new();
    }

    public class BookingPartyCreateVm
    {
        public Guid Id { get; set; }
        public int Index { get; set; }

        [Required]
        public int SequenceNo { get; set; }

        [Required]
        public Guid PartyId { get; set; }

        [Required]
        public int RoleTypeId { get; set; }

        [Required]
        public bool IsPrimaryRole { get; set; }

        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public List<SelectListItem> RoleTypes { get; set; } = new();
    }




    public class JobCompanyRoleCreateVm
    {
        public int Index { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int CompanyRoleTypeId { get; set; }


        [Required]
        public int ServiceTypeId { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> ServiceTypes { get; set; } = new();
        public List<SelectListItem> CompanyRoleTypes { get; set; } = new();
    }









    public class JobPackageCreateVm
    {
        public Guid Id { get; set; }
        public int Index { get; set; }

        [Required]
        public int PackageTypeId { get; set; }

        public int? UnitCounts { get; set; }
        public decimal? UnitLength { get; set; }
        public decimal? UnitWidth { get; set; }
        public decimal? UnitHeight { get; set; }
        public int? DimensionMeasurementUnitId { get; set; }

        public decimal? UnitVolume { get; set; }
        public int? VolumeMeasurementUnitId { get; set; }


        public decimal? UnitGrossWeight { get; set; }
        public decimal? UnitNetWeight { get; set; }
        public int? WeightMeasurementUnitId { get; set; }

        public decimal? TotalLength { get; set; }
        public decimal? TotalWidth { get; set; }
        public decimal? TotalHeight { get; set; }
        public decimal? TotalVolume { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public List<SelectListItem> PackageTypes { get; set; } = new();
        public List<SelectListItem> DimensionMeasurementUnits { get; set; } = new();
        public List<SelectListItem> VolumeMeasurementUnits { get; set; } = new();
        public List<SelectListItem> WeightMeasurementUnits { get; set; } = new();
    }


    public class JobContainerCreateVm
    {
        public Guid Id { get; set; }
        public int Index { get; set; }

        [Required]
        public int ContainerSizeTypeId { get; set; }

        [Required]
        public int? Counts { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }


        public int? cntOwnTypeId { get; set; }

        public List<SelectListItem> SizeTypes { get; set; } = new();
        public List<SelectListItem> CntOwnTypes { get; set; } = new();
    }


    public class JobStatsViewModel

    {
        public int TotalJobs { get; set; }
        public int Draft { get; set; }
        public int Confirmed { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int Closed { get; set; }
        public int Canceled { get; set; }
        public int OnHold { get; set; }

    }

    public class JobDetailTrackingVM
    {
        public JobDetailsVm JobDetails { get; set; }
        public List<BLListItemDto> BlInfos { get; set; }
        public List<RateRequestContainerListItemVm> Containers { get; set; }
        public List<ShipmentLegListItemVm>  Legs { get; set; }
    }

    public class JobNoFileNoVm
    {
        public string JobNumber { get; set; }
        public string JobFileNumber { get; set; }
    }



    public class JobDetailsVm
    {
        public Guid Id { get; set; }
        public int StatusId { get; set; }
        public string JobNumber { get; set; }
        public string? JobFileNumber { get; set; }
        public string? JobSourceTypeName { get; set; }
        public string? Status { get; set; }
        public DateTime? JobDate { get; set; }
        public string? CompanyName { get; set; }
        public string? BranchName { get; set; }
        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }
        public string? CustomerName { get; set; }
        public string? BookingNumber { get; set; }
        public string? ShipmentNumber { get; set; }


        public string? RateRequestNo { get; set; }
        public string? InquiryNo { get; set; }
        public string? BuyQuoteNo { get; set; }
        public string? SellQuoteNo { get; set; }
        public string? CustomerRef { get; set; }
        public string? Incoterm { get; set; }

        public string TransportTypeName { get; set; } = string.Empty;
        public string LoadTypeName { get; set; } = string.Empty;
        public string CargohandlingTypeName { get; set; } = string.Empty;
        public string CargoNatureName { get; set; } = string.Empty;


        public DateTime? CargoReadyDate { get; set; }
        public string MovementTypeName { get; set; } = "";
        public string TradeTypeName { get; set; } = "";




        public string? SalesUserName { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? UpdatedByUserName { get; set; }

        public string ActiveTab { get; set; } = "Overview";

        public List<JobCompanyRoleListItemVm> JobCompanyRolesList { get; set; }
        public IEnumerable<ShipmentLegListItemVm> Legs { get; set; }


    }
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    //Charges Section
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/

    public class JobChargesVm
    {
        public Guid JobId { get; set; }
        public List<ChargeItemVm> SellCharges { get; set; } = new();
        public List<ChargeItemVm> BuyCharges { get; set; } = new();

        public decimal SellTotal => SellCharges.Sum(x => x.Total);
        public decimal BuyTotal => BuyCharges.Sum(x => x.Total);
        public decimal Profit => SellTotal - BuyTotal;
    }
    public class ChargeItemVm
    {
        public Guid Id { get; set; }
        public string Charge { get; set; }
        public string? ChargeDscription { get; set; }
        public string Vendor { get; set; }    // فقط Buy
        public string Currency { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal Total => Qty * Amount;

        public string PartyName { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal UnitRate { get; set; }
        public bool IsTaxable { get; set; }
        public string? Remarks { get; set; }
    }
    public class CreateChargeVm
    {
        public Guid JobId { get; set; }
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public Guid? PartyId { get; set; }     // Vendor
        public string? PartyName { get; set; }
        public int ChargeTypeId { get; set; }
        public int DirectionId { get; set; }
        public string ChargeDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Amount { get; set; }
        public bool Taxable { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public int CurrencyId { get; set; }
        public string Remarks { get; set; }

        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
    }
    public class ChargeDeleteVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string ChargeDescription { get; set; } = null!;
    }
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    //Bookings Section
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/

    public class BookingTabVm
    {
        public Guid JobId { get; set; }
        public List<BookingListItemVm> Bookings { get; set; } = new();
    }


    public class BookingListItemVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }

        public string BookingNo { get; set; } = string.Empty;
        public string LineBookingNo { get; set; }

        public string CompanyName { get; set; }
        public string CompanyRoleTypeName { get; set; }

        public string CarrierName { get; set; }
        public string VendorName { get; set; }

        public string CustomerName { get; set; }

        public string TransportType { get; set; }
        public string TradeType { get; set; }
        public string LoadType { get; set; }

        //public string JobType { get; set; }
        public string JobStatus { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }
        public string MovementType { get; set; }
        public DateTime? BookingDate { get; set; }
        public int BookingStatusId { get; set; }
        public string BookingStatusName { get; set; }

    }

    public class CreateBookingVm
    {
        public Guid JobId { get; set; }
        public Guid? BookingId { get; set; }

        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }


        [Required]
        public int CompanyId { get; set; }
        public int CompanyRoleTypeId { get; set; }

        public int ServiceTypeId { get; set; }



        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; }

        [MaxLength(50)]
        public string LineBookingNo { get; set; }

        public Guid? CustomerId { get; set; }

        [Required]
        public int TransportTypeId { get; set; }

        [Required]
        public int TradeTypeId { get; set; }

        [Required]
        public int LoadTypeId { get; set; }

        [Required]
        public int CargoHandlingTypeId { get; set; }

        [Required]
        public int CargoNatureId { get; set; }

        public DateTime? CargoReadyDate { get; set; }

        //public bool IsConsolMaster { get; set; }

        [Required]
        public Guid OriginLocationId { get; set; }

        [Required]
        public Guid DestinationLocationId { get; set; }

        [Required]
        public int MovementTypeId { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }


        //For Sea
        public Guid? POLId { get; set; }
        public Guid? PODId { get; set; }


        //For Air
        public Guid? FlightId { get; set; }
        public Guid? FlightDateId { get; set; }

        //For Road
        public Guid? PickupLocationId { get; set; }
        public Guid? DeliveryLocationId { get; set; }
        public int? TruckTypeId { get; set; }
        public string? TruckPlate { get; set; } = string.Empty;
        public string? Driver { get; set; } = string.Empty;
        public DateTime? LoadingDate { get; set; }
        public DateTime? UnloadingDate { get; set; }






        public DateTime? CutOffDate { get; set; }
        public DateTime? SICutOffDate { get; set; }
        public DateTime? VGMCutOffDate { get; set; }
        public DateTime? DocumentCutOffDate { get; set; }



        public Guid? CarrierId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? BookingAgentId { get; set; }

        public string? VendorName { get; set; }
        public string? BookingAgentName { get; set; }


        public string? Remarks { get; set; }



        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> LoadTypes { get; set; } = new();
        public List<SelectListItem> CargoHandlingTypes { get; set; } = new();
        public List<SelectListItem> CargoNatures { get; set; } = new();
        public List<SelectListItem> PaymentTypes { get; set; } = new();
        public List<SelectListItem> MovementTypes { get; set; } = new();
        public List<SelectListItem> ServiceTypes { get; set; } = new();
        public List<SelectListItem> CompanyRoleTypes { get; set; } = new();
        public List<SelectListItem> TruckTypes { get; set; } = new();
        public List<SelectListItem> Flights { get; set; } = new();
        public List<SelectListItem> FlightDates { get; set; } = new();
        public List<SelectListItem> Carriers { get; set; } = new();
        public List<SelectListItem> Legs { get; set; } = new();

        ////Dynamin
        public List<JobContainerCreateVm> Containers { get; set; } = new();
        public List<JobPackageCreateVm> Packages { get; set; } = new();
        public List<BookingPartyCreateVm> Parties { get; set; } = new();
    }

    public class RTSInstructionsGridVm
    {
        public Guid BookingId { get; set; }
        public Guid JobId { get; set; }

        public List<RTSInstructionListItemVm> Items { get; set; } = new();
    }
    public class CreateRTSInstructionVm
    {
        public Guid? Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid JobId { get; set; }

        public int CompanyId { get; set; }

        public string RTSInstructionNo { get; set; } = string.Empty;
        public DateTime? RTSInstructionDate { get; set; }

        [Required]
        public Guid? ShipperId { get; set; }
        [Required]
        public Guid? CarrierId { get; set; }

        [Required]
        public Guid? DepotId { get; set; }
        public int? StatusId { get; set; }

        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }

        public string? Remarks { get; set; }

        public List<SelectListItem>? Shippers { get; set; } = new();
        public List<SelectListItem>? Carriers { get; set; } = new();
        public List<SelectListItem>? Depots { get; set; } = new();
        public List<SelectListItem>? Statuses { get; set; } = new();

        public List<SelectListItem>? ContainerSizeTypes { get; set; } = new();

        public List<RTSContainerInputVm>? Containers { get; set; } = new();
    }


    public class RTSInstructionListItemVm
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid JobId { get; set; }

        public string RTSInstructionNo { get; set; } = string.Empty;
        public DateTime? RTSInstructionDate { get; set; }

        public string ShipperName { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string DepotName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;

        public string CreateUserName { get; set; } = string.Empty;
        public string UpdateUserName { get; set; } = string.Empty;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }

        public string? Remarks { get; set; }

        public List<RTSInstructionContainerTypeVm> ContainerTypes { get; set; } = new();


        // پراپرتی محاسباتی برای ساخت خلاصه کانتینرها
        public string ContainerSummary
        {
            get
            {
                if (ContainerTypes == null || !ContainerTypes.Any())
                    return "-";

                // فیلتر کردن ردیف‌های نامعتبر یا بدون تعداد
                var summaryParts = ContainerTypes
                    .Where(c => c.ContainerCounts.HasValue && c.ContainerCounts > 0 && !string.IsNullOrWhiteSpace(c.ContainerSizeTypeName))
                    .Select(c => $"{c.ContainerCounts}X{c.ContainerSizeTypeName?.ToUpper().Trim()}");

                return summaryParts.Any() ? string.Join(" + ", summaryParts) : "-";
            }
        }
    }

    public class RTSInstructionContainerTypeVm
    {
        public Guid Id { get; set; }
        public Guid RTSInstructionId { get; set; }

        public int? ContainerSizeTypeId { get; set; }
        public string? ContainerSizeTypeName { get; set; }

        public int? ContainerCounts { get; set; }

        public string? Remarks { get; set; } = string.Empty;
    }


    public class RTSContainerInputVm
    {
        public int ContainerSizeTypeId { get; set; }
        public string? ContainerSizeTypeName { get; set; } // برای نمایش
        public int MaxAvailableCount { get; set; }      // تعداد موجود در بوکینگ

        // مقادیری که کاربر وارد می‌کند:
        public int RequestedCount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class RTSInstructionDetailsVm
    {
        public Guid Id { get; set; }
        public string RTSInstructionNo { get; set; } = string.Empty;
        public DateTime? RTSInstructionDate { get; set; }

        public string? RTSInstructionLocalDate { get; set; }


        public string? ShipperName { get; set; }
        public string? ShipperLocalName { get; set; }


        public string? CarrierName { get; set; }
        public string? CarrierLocalName { get; set; }

        public string? DepotName { get; set; }
        public string? StatusName { get; set; }

        public DateTime? ValidDateFrom { get; set; }
        public DateTime? ValidDateTo { get; set; }

        public string? Remarks { get; set; }

        public List<RTSInstructionDetailsContainerVm> Containers { get; set; } = new();
    }

    public class RTSInstructionDetailsContainerVm
    {
        public Guid? Id { get; set; }
        public Guid? RTSInstructionId { get; set; }

        public string ContainerSizeTypeName { get; set; } = string.Empty;
        public int ContainerCounts { get; set; }
        public string? Remarks { get; set; }
    }

    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    //BLs Section
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
    public class BLListItemVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid? BookingId { get; set; }

        public string BookingNo { get; set; } = string.Empty;
        public string CompanyName { get; set; }
        public string CompanyRoleTypeName { get; set; }

        public string CarrierName { get; set; }
        public string VendorName { get; set; }

        public string CustomerName { get; set; }

        public string TransportType { get; set; }
        public string TradeType { get; set; }
        public string LoadType { get; set; }

        //public string JobType { get; set; }
        public string JobStatus { get; set; }

        public string Origin { get; set; }
        public string Destination { get; set; }
        public string MovementType { get; set; }
        public DateTime? BookingDate { get; set; }
    }
    public class CreateBLVm
    {
        public Guid JobId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? BLId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingNo { get; set; }

        [MaxLength(50)]
        public string LineBookingNo { get; set; }

        public Guid? CustomerId { get; set; }

        [Required]
        public int TransportTypeId { get; set; }

        [Required]
        public int TradeTypeId { get; set; }

        [Required]
        public int LoadTypeId { get; set; }

        [Required]
        public int CargoHandlingTypeId { get; set; }

        [Required]
        public int CargoNatureId { get; set; }

        public DateTime? CargoReadyDate { get; set; }

        //public bool IsConsolMaster { get; set; }

        [Required]
        public Guid OriginLocationId { get; set; }

        [Required]
        public Guid DestinationLocationId { get; set; }

        [Required]
        public int MovementTypeId { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Customers { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> LoadTypes { get; set; } = new();
        public List<SelectListItem> CargoHandlingTypes { get; set; } = new();
        public List<SelectListItem> CargoNatures { get; set; } = new();
        public List<SelectListItem> PaymentTypes { get; set; } = new();
        public List<SelectListItem> MovementTypes { get; set; } = new();

        ////Dynamin
        //public List<JobContainerCreateVm> Containers { get; set; } = new();
        //public List<JobPackageCreateVm> Packages { get; set; } = new();
        //public List<JobPartyCreateVm> JobParties { get; set; } = new();
    }
    /*******************************************************************************************************************************************************************************************************************************/
    /*******************************************************************************************************************************************************************************************************************************/
}
