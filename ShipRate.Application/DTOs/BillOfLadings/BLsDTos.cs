namespace ShipRate.Application.DTOs.BillOfLadings
{
    public class BLContextDto
    {
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
    }


    public class BLLookupDto
    {
        public List<LookupItemDto> Companies { get; set; } = new();
        public List<LookupItemDto> BLTypes { get; set; } = new();
        public List<LookupItemDto> BLLevels { get; set; } = new();
        public List<LookupItemDto> TransportTypes { get; set; } = new();
        public List<LookupItemDto> LoadTypes { get; set; } = new();
        public List<LookupItemDto> MovementTypes { get; set; } = new();
        public List<LookupItemDto> BLStatuses { get; set; } = new();
        public List<LookupItemDto> PaymentTypes { get; set; } = new();

        public List<LookupItemDto> Locations { get; set; } = new();
        public List<LookupItemDto> Persons { get; set; } = new();
        public List<LookupItemDto> BLPartyTypes { get; set; } = new();

        public List<LookupItemDto> Commodities { get; set; } = new();
        public List<LookupItemDto> PackageTypes { get; set; } = new();

        public List<LookupItemDto> ContainerUses { get; set; } = new();
        public List<LookupItemDto> ContainerSizeTypes { get; set; } = new();

        public List<LookupItemDto> VoyageTypes { get; set; } = new();
        public List<LookupItemDto> Voyages { get; set; } = new();
        public List<LookupItemDto> Trips { get; set; } = new();

        public List<LookupItemDto> Carriers { get; set; } = new();
        public List<LookupItemDto> IssueParties { get; set; } = new();


    }

    public class LookupItemDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }



    public class BLPartyDto
    {
        public Guid? Id { get; set; }

        public int RoleTypeId { get; set; }
        public Guid? PartyId { get; set; }

        public string? PartyTextInBL { get; set; }
        public string? PartyNationalNo { get; set; }
        public string? PartyTelephone { get; set; }
        public string? PartyMobile { get; set; }
        public string? PartyFax { get; set; }
        public string? PartyEmail { get; set; }

        public bool IsConsigneeToOrder { get; set; }
        public bool IsConsigneeToOrderOf { get; set; }
        public bool IsConsigneeToBearer { get; set; }
        public bool IsNotifySameAs { get; set; }
    }

    public class CargoDetailDto
    {
        public Guid? Id { get; set; }

        public int? Sequence { get; set; }
        public int? PackageTypeId { get; set; }
        public int? PackageCount { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Volume { get; set; }

        public string? CargoDescription { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? HSCodeToSave { get; set; }
        public string? Remarks { get; set; }

        public bool IsDG { get; set; }
        public bool IsOverSized { get; set; }
        public bool IsTemperatureControlled { get; set; }
        public bool IsFragile { get; set; }
        public bool IsPerishable { get; set; }
        public bool IsStackable { get; set; }
    }

    public class BLContainerDto
    {
        public Guid? Id { get; set; }

        public Guid ContainerUseId { get; set; }
        public string? ContainerNumber { get; set; }
        public int ContainerSizeTypeId { get; set; }

        public int? TotalPackageCount { get; set; }

        public string? SealNumber { get; set; }
        public string? SealNumber2 { get; set; }

        public decimal? TotalVolumeCBM { get; set; }
        public decimal? BLContainerPercent { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }

        public int? DimensionUnitId { get; set; }
        public decimal? OverHeight { get; set; }
        public decimal? OverLength { get; set; }
        public decimal? OverWidth { get; set; }

        public List<CargoDetailDto> CargoDetails { get; set; } = new();
    }

    public class BLVoyageDto
    {
        public Guid? Id { get; set; }

        public int VoyageTypeId { get; set; }
        public Guid? VoyageId { get; set; }

        public string? VoyageNo { get; set; }
        public string? VesselName { get; set; }

        public DateTime? ReceivedForShipmentDate { get; set; }
        public DateTime? ShippedOnBoardDate { get; set; }
    }

    public class BLFlightDto
    {
        public Guid? Id { get; set; }

        public Guid? FlightId { get; set; }
        public Guid? FlightDateId { get; set; }

        public string? FlightNo { get; set; }
        public Guid? AirlineId { get; set; }

        public string? DeclaredValueForCarriage { get; set; }
        public string? DeclaredValueForCustoms { get; set; }
        public DateTime? FlightDateDate { get; set; }

        public string? HandlingInformation { get; set; }
    }

    public class BLTripDto
    {
        public Guid? Id { get; set; }

        public Guid? TripId { get; set; }

        public DateTime? LoadingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string? DriverName { get; set; }
        public string? DriverPassportNo { get; set; }
        public string? TruckNo { get; set; }
        public string? TrailerNo { get; set; }
    }

    public class BLRailJourneyDto
    {
        public Guid? Id { get; set; }

        public string? TrainNo { get; set; }
        public string? WagonNo { get; set; }

        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
    }
    public class BLUpsertDto
    {
        public Guid? Id { get; set; }

        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public int CompanyId { get; set; }
        public int BLLevelId { get; set; }
        public int TransportTypeId { get; set; }

        public string BLNo { get; set; } = string.Empty;
        public DateTime BLIssueDate { get; set; } = DateTime.Today;
        public string IssuePlace { get; set; } = string.Empty;

        public Guid? CarrierId { get; set; }
        public int? BLTypeId { get; set; }

        public int OriginalCount { get; set; }
        public int CopyCount { get; set; }

        public int LoadTypeId { get; set; }
        public int MovementTypeId { get; set; }
        public int BLStatusId { get; set; }
        public int PaymentTypeId { get; set; }

        public bool IssuedByUs { get; set; }
        public bool IsSwitched { get; set; }
        public bool IsContainerized { get; set; }

        public Guid? IssuerPartyId { get; set; }

        public Guid PORId { get; set; }
        public string? PORText { get; set; }

        public Guid POLId { get; set; }
        public string? POLText { get; set; }

        public Guid PODId { get; set; }
        public string? PODText { get; set; }

        public Guid PODelId { get; set; }
        public string? PODelText { get; set; }

        public string? FinalDestinationText { get; set; }

        public Guid? FreightPrepaidATId { get; set; }
        public string? FreightPrepaidATText { get; set; }

        public Guid? PointandCountryOfOriginId { get; set; }
        public string? PointandCountryOfOriginText { get; set; }

        public Guid? OnwardInlandRoutingId { get; set; }
        public string? OnwardInlandRoutingText { get; set; }

        public string? GoodsDescription { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? RemarksInBL { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public decimal? ChargeableWeight { get; set; }

        public int? TotalContainers { get; set; }
        public int? TotalPackages { get; set; }

        public int? PackageTypeId { get; set; }
        public string? PackageTypeText { get; set; }

        public string? ExportReference { get; set; }
        public string? ForwardingAgentReference { get; set; }
        public string? SerialNo { get; set; }
        public string? MRN { get; set; }
        public string? PursuitCode { get; set; }
        public string? FreightTerms { get; set; }
        public string? LCNo { get; set; }
        public string? LCBankName { get; set; }
        public string? OrderRegistrationNo { get; set; }

        public DateTime? ShippedOnBoardDate { get; set; }
        public DateTime? ReceivedForShipmentDate { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public int? BLReleaseTypeId { get; set; }
        public Guid? SalesPersonId { get; set; }
        public Guid? TranshipmentPortId { get; set; }

        public string? ClauseRemarks { get; set; }
        public string? OnBoardNotation { get; set; }
        public string? RemarksInManifest { get; set; }
        public string? InternalNotes { get; set; }

        public List<BLPartyDto> Parties { get; set; } = new();
        public List<BLContainerDto> Containers { get; set; } = new();
        public List<CargoDetailDto> CargoDetails { get; set; } = new();

        public List<BLVoyageDto> Voyages { get; set; } = new();
        public List<BLFlightDto> Flights { get; set; } = new();
        public List<BLTripDto> Trips { get; set; } = new();
        public List<BLRailJourneyDto> RailJourneys { get; set; } = new();


        // UI-only
        public string? ListTargetId { get; set; }
        public string? ModalId { get; set; }

    }

    public sealed class BLFormVm
    {
        public BLUpsertDto Form { get; set; } = new();
        public BLLookupDto Lookups { get; set; } = new();

        public string? ListTargetId { get; set; }
        public string? ModalId { get; set; }
    }

}
