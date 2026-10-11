using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.BillOfLadings;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class BLListItemDto
    {
        public Guid Id { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public DateTime BLIssueDate { get; set; }

        public string TransportTypeName { get; set; } = string.Empty;
        public string BLTypeName { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;

        public string POLText { get; set; } = string.Empty;
        public string PODText { get; set; } = string.Empty;

        public int? TradeTypeId { get; set; }
        public string? TradeTypeName { get; set; }

        public int TotalContainers { get; set; }
        public int TotalPackages { get; set; }
    }

    public class BLManagePageVm
    {
        public BLContextDto Context { get; set; } = new();
        public List<BLListItemDto> Items { get; set; } = new();
    }

    public class TransportDocumentUpsertVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid JobId { get; set; }

        public string? JobNo { get; set; }

        public Guid? BookingId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public int? CompanyId { get; set; }
        public int? IncotermId { get; set; }

        // Main BL info
        [Display(Name = "Transport Type")]
        public int? TransportTypeId { get; set; }

        [Display(Name = "BL Type")]
        public int? BLTypeId { get; set; }

        [Display(Name = "BL Level")]
        public int? BLLevelId { get; set; }

        [Display(Name = "BL No")]
        [MaxLength(100)]
        public string? BLNo { get; set; }

        [Display(Name = "Master BL No")]
        [MaxLength(100)]
        public string? MasterBLNo { get; set; }


        [Display(Name = "Issue Date")]
        public DateTime? BLIssueDate { get; set; }

        [Display(Name = "Place Of Issue")]
        [MaxLength(250)]
        public string? PlaceOfIssue { get; set; }

        public Guid? CarrierId { get; set; }
        public Guid? IssuerPartyId { get; set; }
        public string? IssuerPartyName { get; set; }

        public bool IssuedByUs { get; set; }

        public int? BLReleaseTypeId { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public int? OriginalCount { get; set; }
        public int? CopyCount { get; set; }

        public int? BLStatusId { get; set; }
        public string? StatusName { get; set; }

        public string? ExportReference { get; set; }
        public string? ForwardingAgentReference { get; set; }
        public string? SerialNo { get; set; }

        public bool IsSwitched { get; set; }

        // Parties
        public Guid? ShipperPartyId { get; set; }
        public string? ShipperPartyName { get; set; }
        public string? ShipperTextInBL { get; set; }

        public Guid? ConsigneePartyId { get; set; }
        public string? ConsigneePartyName { get; set; }
        public string? ConsigneeTextInBL { get; set; }

        public bool IsConsigneeToOrder { get; set; }
        public bool IsConsigneeToOrderOf { get; set; }
        public bool IsConsigneeToBearer { get; set; }

        public Guid? NotifyPartyId { get; set; }
        public string? NotifyPartyName { get; set; }
        public string? NotifyPartyTextInBL { get; set; }

        public bool IsNotifySameAsConsignee { get; set; }

        public Guid? AgentPartyId { get; set; }
        public string? AgentPartyName { get; set; }
        public string? AgentTextInBL { get; set; }

        //Routes / Locations

        public Guid? PORId { get; set; }
        public string? PORName { get; set; }
        public string? PORText { get; set; }

        public Guid? POLId { get; set; }
        public string? POLName { get; set; }
        public string? POLText { get; set; }

        public Guid? PODId { get; set; }
        public string? PODName { get; set; }
        public string? PODText { get; set; }

        public Guid? PODelId { get; set; }
        public string? PODelName { get; set; }
        public string? PODelText { get; set; }

        public string? FinalDestinationText { get; set; }

        public Guid? TranshipmentPortId { get; set; }
        public string? TranshipmentPortName { get; set; }
        public string? TranshipmentPortText { get; set; }

        public Guid? PointandCountryOfOriginId { get; set; }
        public string? PointandCountryOfOriginText { get; set; }

        public Guid? OnwardInlandRoutingId { get; set; }
        public string? OnwardInlandRoutingText { get; set; }

        public int? TradeTypeId { get; set; }

        // Cargo
        public int? LoadTypeId { get; set; }
        public int? MovementTypeId { get; set; }

        public string? MarksAndNumbers { get; set; }
        public string? GoodsDescription { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public decimal? ChargeableWeight { get; set; }

        public int? TotalPackages { get; set; }

        public int? PackageTypeId { get; set; }
        public string? PackageTypeName { get; set; }
        public string? PackageTypeText { get; set; }

        public int? TotalContainers { get; set; }

        // Freight / Payment
        public int? PaymentTypeId { get; set; }
        public string? FreightTerms { get; set; }

        public Guid? FreightPaymentAtId { get; set; }
        public string? FreightPaymentAtText { get; set; }

        // Dates / Notations
        public DateTime? ShippedOnBoardDate { get; set; }
        public DateTime? ReceivedForShipmentDate { get; set; }

        public string? ClauseRemarks { get; set; }
        public string? OnBoardNotation { get; set; }
        public string? RemarksInBL { get; set; }
        public string? RemarksInManifest { get; set; }
        public string? InternalNotes { get; set; }

        // Customs / Banking
        public string? MRN { get; set; }
        public string? PursuitCode { get; set; }

        public string? LCNo { get; set; }
        public string? LCBankName { get; set; }
        public string? OrderRegistrationNo { get; set; }

        public Guid? POLDetentionTariffId { get; set; }
        public Guid? PODDetentionTariffId { get; set; }

        // Summary section
        public string? SummaryShipperName { get; set; }
        public string? SummaryConsigneeName { get; set; }
        public string? SummaryRoute { get; set; }
        public string? SummaryCargo { get; set; }
        public string? SummaryContainers { get; set; }
        public decimal SummaryTotalCharges { get; set; }




        // Lookups for View
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> BLTypes { get; set; } = new();
        public List<SelectListItem> BLLevels { get; set; } = new();
        public List<SelectListItem> BLReleaseTypes { get; set; } = new();
        public List<SelectListItem> LoadTypes { get; set; } = new();
        public List<SelectListItem> MovementTypes { get; set; } = new();
        public List<SelectListItem> PaymentTypes { get; set; } = new();
        public List<SelectListItem> PackageTypes { get; set; } = new();
        public List<SelectListItem> BLStatuses { get; set; } = new();
        public List<SelectListItem> Parties { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> Carriers { get; set; } = new();
        public List<SelectListItem> Airlines { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> TradeTypes { get; set; } = new();
        public List<SelectListItem> VoyageTypes { get; set; } = new();


        public List<SelectListItem> WeightUnits { get; set; } = new();
        public List<SelectListItem> VolumeUnits { get; set; } = new();
        public List<SelectListItem> DimensionUnits { get; set; } = new();
        public List<SelectListItem> TempUnits { get; set; } = new();

        public List<SelectListItem> CountryHSCodes { get; set; } = new();
        public List<SelectListItem> CargoCommodities { get; set; } = new();

        public List<SelectListItem> DGClasses { get; set; } = new();
        public List<SelectListItem> DGPackingGroups { get; set; } = new();

        public List<SelectListItem> Incoterms { get; set; } = new();
        public List<SelectListItem> ContainerUses { get; set; } = new();
        public List<SelectListItem> ContainerSizeTypes { get; set; } = new();
        public List<SelectListItem> FullMTs { get; set; } = new();
        public List<SelectListItem> CntOwnTypes { get; set; } = new();

        public List<SelectListItem> DetentionTariffs { get; set; } = new();

        public List<BLVoyageRowVm> Voyages { get; set; } = new();
        public List<BLFlightRowVm> Flights { get; set; } = new();
        public List<BLTripRowVm> Trips { get; set; } = new();
        public List<BLRailJourneyRowVm> RailJourneys { get; set; } = new();

        public List<CargoDetailVm> CargoDetails { get; set; } = new();

        public List<BLContainerVm> Containers { get; set; } = new();



    }

    public class BLContainerVm
    {
        public Guid Id { get; set; }
        public Guid BLId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public int? ContainerSizeTypeId { get; set; }
        public int? DimensionUnitId { get; set; }
        public int? FullMTId { get; set; }
        public int? CntOwnTypeId { get; set; }
        public int? LoadTypeId { get; set; }


        public int? Seq { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public string? SealNumber { get; set; }
        public string? SealNumber2 { get; set; }
        public string? ExportPermission { get; set; }

        public int TotalPackageCount { get; set; }

        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }
        public decimal? TotalVolumeCBM { get; set; }
        public decimal? TareWeight { get; set; }


        public decimal? BLContainerPercent { get; set; }

        public decimal? OverLength { get; set; }
        public decimal? OverWidth { get; set; }
        public decimal? OverHeight { get; set; }

        public string? ContainerTypeName { get; set; }
        public string? ContainerUseName { get; set; }
        public string? DimensionUnitName { get; set; }

        public List<BLContainerCargoAllocationVm> CargoAllocations { get; set; } = new();
    }

    public class BLContainerCargoAllocationVm
    {
        public Guid? Id { get; set; }

        public Guid? BLContainerId { get; set; }
        public Guid CargoDetailId { get; set; }

        public string? CargoDetailDescription { get; set; }
        public string? CargoDetailMarks { get; set; }

        public string? CargoDisplayName { get; set; }


        public decimal? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Volume { get; set; }

        public string? Remarks { get; set; }
    }


    public class CargoDetailVm
    {
        public Guid? Id { get; set; }
        public Guid? BLId { get; set; }

        [Display(Name = "Sequence")]
        public int Sequence { get; set; }

        // Package
        [Display(Name = "Package Type")]
        public int? PackageTypeId { get; set; }

        [Display(Name = "Package Count")]
        public decimal? PackageCount { get; set; }

        // Weight
        [Display(Name = "Gross Weight")]
        public decimal? GrossWeight { get; set; }

        [Display(Name = "Net Weight")]
        public decimal? NetWeight { get; set; }

        [Display(Name = "Chargeable Weight")]
        public decimal? ChargeableWeight { get; set; }

        [Display(Name = "Weight Unit")]
        public int? WeightUnitId { get; set; }

        // Volume
        [Display(Name = "Volume")]
        public decimal? Volume { get; set; }

        [Display(Name = "Volume Unit")]
        public int? VolumeUnitId { get; set; }

        // Dimensions / Oversized
        [Display(Name = "Oversized")]
        public bool IsOverSized { get; set; }

        [Display(Name = "Length")]
        public decimal? Length { get; set; }

        [Display(Name = "Width")]
        public decimal? Width { get; set; }

        [Display(Name = "Height")]
        public decimal? Height { get; set; }

        [Display(Name = "Dimension Unit")]
        public int? DimensionUnitId { get; set; }

        // HS Code / Commodity
        [Display(Name = "Country HS Code")]
        public Guid? CountryHSCodeId { get; set; }

        [Display(Name = "HS Code")]
        [MaxLength(100)]
        public string? HSCodeToSave { get; set; } = string.Empty;

        [Display(Name = "Commodity")]
        public Guid? CargoCommodityId { get; set; }

        // Dangerous Goods
        [Display(Name = "Dangerous Goods")]
        public bool IsDG { get; set; }

        [Display(Name = "DG Class")]
        public int? DGClassId { get; set; }

        [Display(Name = "UN Number")]
        [MaxLength(50)]
        public string? UNNumber { get; set; }

        [Display(Name = "Flash Point")]
        public decimal? DGFlashPoint { get; set; }

        [Display(Name = "Packing Group")]
        public int? DGPackingGroupId { get; set; }

        // Temperature Controlled
        [Display(Name = "Temperature Controlled")]
        public bool IsTemperatureControlled { get; set; }

        [Display(Name = "Min Temp")]
        public decimal? MinTemp { get; set; }

        [Display(Name = "Normal Temp")]
        public decimal? NormalTemp { get; set; }

        [Display(Name = "Max Temp")]
        public decimal? MaxTemp { get; set; }

        [Display(Name = "Temperature Unit")]
        public int? TempUnitId { get; set; }

        // Cargo Characteristics
        [Display(Name = "Stackable")]
        public bool IsStackable { get; set; }

        [Display(Name = "Rotateable")]
        public bool IsRotateable { get; set; }

        [Display(Name = "Perishable")]
        public bool IsPerishable { get; set; }

        [Display(Name = "Fragile")]
        public bool IsFragile { get; set; }

        // Texts
        [Display(Name = "Cargo Description")]
        public string CargoDescription { get; set; } = string.Empty;

        [Display(Name = "Marks And Numbers")]
        public string? MarksAndNumbers { get; set; } = string.Empty;

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        // Optional display-only fields for grid/edit view
        public string? PackageTypeName { get; set; }
        public string? WeightUnitName { get; set; }
        public string? VolumeUnitName { get; set; }
        public string? DimensionUnitName { get; set; }
        public string? TempUnitName { get; set; }
        public string? DGClassName { get; set; }
        public string? DGPackingGroupName { get; set; }
        public string? CargoCommodityName { get; set; }
        public string? CountryHSCodeText { get; set; }
    }
    public static class BLPartyRoleIds
    {
        public const int Shipper = 51;
        public const int Consignee = 52;
        public const int Notify = 53;
        public const int Agent = 3;
    }
    public enum BLPartyRole
    {
        Shipper = 51,
        Consignee = 52,
        Notify = 53,
        Agent = 3
    }

    public class BLVoyageRowVm
    {
        public Guid? Id { get; set; }
        public int? VoyageTypeId { get; set; }
        public bool IsFinal { get; set; }
        public Guid? VoyageId { get; set; }
        public string? SelectedVoyageText { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }
        public DateTime? ShippedOnBoardDate { get; set; }
        public DateTime? ReceivedForShipmentDate { get; set; }
    }

    public class BLFlightRowVm
    {
        public Guid? Id { get; set; }

        public Guid? AirlineId { get; set; }
        public Guid? FlightId { get; set; }
        public Guid? FlightDateId { get; set; }

        // فقط برای نمایش کنترل Lookup
        public string? SelectedFlightText { get; set; }
        public string? SelectedFlightDateText { get; set; }

        public string? FlightNo { get; set; }

        public DateTime? FlightDateDate { get; set; }
        public string? FlightDateLocal { get; set; }
        public string? AirlineName { get; set; }

        public string? DeclaredValueForCarriage { get; set; }
        public string? DeclaredValueForCustoms { get; set; }
        public string? HandlingInformation { get; set; }
    }

    public class BLTripRowVm
    {
        public Guid? Id { get; set; }

        public Guid? TripId { get; set; }

        // فقط برای نمایش در input انتخاب‌شده؛ در BLTrip ذخیره نمی‌شود.
        public string? SelectedTripText { get; set; }

        public string? TruckNo { get; set; }
        public string? TrailerNo { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPassportNo { get; set; }

        public int? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public int? AllocatedContainersCount { get; set; }


        public DateTime? LoadingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string? CMRNo { get; set; }
        public string? TIRNo { get; set; }
        public string? ManifestNo { get; set; }
    }

    public class BLRailJourneyRowVm
    {
        public Guid? Id { get; set; }
        public string? TrainNo { get; set; }
        public string? WagonNo { get; set; }
    }




    public enum BLSplitSourceLineType
    {
        ContainerAllocation = 1,
        DirectCargo = 2
    }


    public class SubmitAdvancedSplitVm
    {
        public Guid SplitRequestId { get; set; }
        public Guid SourceBlId { get; set; }
        public Guid JobId { get; set; }
        public List<SubmitAdvancedSplitTargetVm> Targets { get; set; } = new();
    }

    public class SubmitAdvancedSplitTargetVm
    {
        public string TargetKey { get; set; } = string.Empty;
        public string? TargetBlNo { get; set; }
        public List<SubmitAdvancedSplitAssignmentVm> Assignments { get; set; } = new();
    }

    public class SubmitAdvancedSplitAssignmentVm
    {
        public string SourceLineKey { get; set; } = string.Empty;
        public int SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }

    public class AdvancedBLSplitVm
    {
        public Guid BlId { get; set; }
        public string BlNo { get; set; } = string.Empty;

        public Guid JobId { get; set; }
        public string JobNo { get; set; } = string.Empty;

        public Guid? SplitRequestId { get; set; }
        public Guid? ServiceRequestId { get; set; }





        public bool IsContainerized { get; set; }

        public List<BLSplitSourceLineVm> SourceLines { get; set; } = new();
        public List<BLSplitTargetVm> Targets { get; set; } = new();
        public BLSplitTotalsVm Totals { get; set; } = new();
    }


    public class BLSplitSourceLineVm
    {
        public string SourceLineKey { get; set; } = string.Empty;
        public BLSplitSourceLineType SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public Guid? BlContainerId { get; set; }
        public string ContainerNo { get; set; } = string.Empty;

        public string CargoNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal AvailablePackages { get; set; }
        public decimal AvailableWeight { get; set; }
        public decimal AvailableVolume { get; set; }
    }

    public class BLSplitTargetVm
    {
        public string TargetKey { get; set; } = string.Empty;
        public string? TargetBlNo { get; set; }
        public string Title { get; set; } = string.Empty;

        public List<BLSplitAssignmentVm> Assignments { get; set; } = new();
        public BLSplitTotalsVm Totals { get; set; } = new();
    }

    public class BLSplitAssignmentVm
    {
        public string SourceLineKey { get; set; } = string.Empty;
        public BLSplitSourceLineType SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public string ContainerNo { get; set; } = string.Empty;
        public string CargoNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }

    public class BLSplitTotalsVm
    {
        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }










    public class SplitAllocationVm
    {
        public Guid AllocationId { get; set; }
        public string ContainerNo { get; set; } = string.Empty;
        public int Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }

    public class SubmitAdvancedBLSplitRequestVm
    {
        public Guid SourceBlId { get; set; }
        public Guid JobId { get; set; }

        public List<SubmitBLSplitTargetVm> Targets { get; set; } = new();
    }

    public class SubmitBLSplitTargetVm
    {
        public string TargetKey { get; set; } = string.Empty;
        public string? TargetBlNo { get; set; }

        public List<SubmitBLSplitAssignmentVm> Assignments { get; set; } = new();
    }

    public class SubmitBLSplitAssignmentVm
    {
        public string SourceLineKey { get; set; } = string.Empty;
        public BLSplitSourceLineType SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }

    public class SubmitAdvancedBLSplitVm
    {
        public Guid SourceBlId { get; set; }
        public Guid JobId { get; set; }

        public List<SubmitAdvancedBLSplitTargetVm> Targets { get; set; } = new();
    }

    public class SubmitAdvancedBLSplitTargetVm
    {
        public string TargetKey { get; set; } = "";
        public string? TargetBlNo { get; set; }

        public List<SubmitAdvancedBLSplitAssignmentVm> Assignments { get; set; } = new();
    }

    public class SubmitAdvancedBLSplitAssignmentVm
    {
        public string SourceLineKey { get; set; } = "";
        public int SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }


    public class SubmitAdvancedSplitResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ServiceRequestId { get; set; }
        public Guid? SplitRequestId { get; set; }

        public static SubmitAdvancedSplitResult Ok(Guid serviceRequestId, Guid splitRequestId, string message) => new()
        {
            Success = true,
            Message = message,
            ServiceRequestId = serviceRequestId,
            SplitRequestId = splitRequestId
        };

        public static SubmitAdvancedSplitResult Fail(string message) => new()
        {
            Success = false,
            Message = message
        };
    }



    public class ContainerAllocationModalVm
    {
        public Guid BLId { get; set; }
        public Guid RoadTripId { get; set; }
        public string? TruckNo { get; set; }
        public string? DriverName { get; set; }
        public List<ContainerAllocationItemVm> AvailableContainers { get; set; } = new();
    }

    public class ContainerAllocationItemVm
    {
        public Guid ContainerId { get; set; }
        public string ContainerNo { get; set; } = string.Empty;
        public string? ContainerTypeName { get; set; }
        public decimal GrossWeight { get; set; }
        public int PackageCount { get; set; }
        public bool IsAllocatedToThisTrip { get; set; }
        public string? AllocatedToOtherTruck { get; set; } // اگر به تریلی دیگری تخصیص داده شده باشد
    }
    public class SaveContainerAllocationDto
    {
        public Guid BLId { get; set; }
        public Guid RoadTripId { get; set; }
        public List<Guid> SelectedContainerIds { get; set; } = new();
    }
    public class FinalVoyageVm
    {
        public Guid BLId { get; set; }
        public Guid? BLVoyageId { get; set; }
        public Guid? VoyageId { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? BerthDate { get; set; }
        public DateTime? DischargeStartDate { get; set; }
        public DateTime? DischargeCompleteDate { get; set; }
        public Guid? PortId { get; set; }
        public string? PortName { get; set; }
        public string? Message { get; set; }
    }
}