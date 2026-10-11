using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BL : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid JobId { get; set; }


        public int BLTypeId { get; set; }
        public int BLLevelId { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string? MasterBLNo { get; set; }


        public DateTime? BLIssueDate { get; set; }
        public DateTime? MasterBLIssueDate { get; set; }

        public string? PlaceOfIssue { get; set; }
        public string? MasterPlaceOfIssue { get; set; }

        public Guid? CarrierId { get; set; }

        public Guid? IssuerPartyId { get; set; }
        public bool IssuedByUs { get; set; }


        public int TransportTypeId { get; set; }

        public int? TradeTypeId { get; set; }

        public int? OriginalCount { get; set; } = 3;
        public int? CopyCount { get; set; } = 3;

        public int? BLReleaseTypeId { get; set; }
        public int? MasterBLReleaseTypeId { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public DateTime? MasterReleaseDate { get; set; }

        public int? LoadTypeId { get; set; }
        public int? MovementTypeId { get; set; }
        public string? ExportReference { get; set; } = string.Empty;
        public string? ForwardingAgentReference { get; set; } = string.Empty;
        public string? SerialNo { get; set; } = string.Empty;
        public bool IsSwitched { get; set; } = false;

        public Guid? PORId { get; set; }
        public string PORText { get; set; } = string.Empty;
        public Guid? POLId { get; set; }
        public string POLText { get; set; } = string.Empty;
        public Guid? PODId { get; set; }
        public string PODText { get; set; } = string.Empty;
        public Guid? PODelId { get; set; }
        public string PODelText { get; set; } = string.Empty;
        public string FinalDestinationText { get; set; } = string.Empty;

        public string? GoodsDescription { get; set; } = string.Empty;
        public string? MarksAndNumbers { get; set; } = string.Empty;


        public int? PaymentTypeId { get; set; }
        public int? MasterPaymentTypeId { get; set; }


        public string? FreightTerms { get; set; }   //-- free text on BL
        public Guid? FreightPaymentAtId { get; set; }
        public string? FreightPaymentAtText { get; set; }

        public Guid? PointandCountryOfOriginId { get; set; }
        public string? PointandCountryOfOriginText { get; set; } = string.Empty;
        public Guid? OnwardInlandRoutingId { get; set; }
        public string? OnwardInlandRoutingText { get; set; } = string.Empty;

        public string? RemarksInBL { get; set; } = string.Empty;
        public string? RemarksInManifest { get; set; } = string.Empty;

        //public bool IsOriginal { get; set; }
        //public bool IsTelexReleased { get; set; }
        //public bool IsNegotiable { get; set; }
        //public bool IsSurrendered { get; set; }
        //public DateTime? SurrenderDate { get; set; }

        public string? MRN { get; set; } = string.Empty;
        public string? PursuitCode { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public int TotalContainers { get; set; }
        public int TotalPackages { get; set; }
        public int? PackageTypeId { get; set; }
        public string? PackageTypeText { get; set; }

        public DateTime? ShippedOnBoardDate { get; set; }
        public DateTime? ReceivedForShipmentDate { get; set; }

        public DateTime? LastPrintedAt { get; private set; }

        public int BLStatusId { get; set; }



        public Guid? SalesPersonId { get; set; }

        public int? IncotermId { get; set; }

        public string? LCNo { get; set; }
        public string? LCBankName { get; set; }

        public string? OrderRegistrationNo { get; set; }
        public Guid? TranshipmentPortId { get; set; }

        //Clauses
        public string? ClauseRemarks { get; set; }
        public string? OnBoardNotation { get; set; }




        public string? InternalNotes { get; set; }

        public DateTime? BLDeletedAt { get; set; }
        public Guid? BLDeletedByUserId { get; set; }


        public Guid? POLDetentionTariffId { get; set; }
        public Guid? PODDetentionTariffId { get; set; }


        public Guid? MasterPOLDetentionTariffId { get; set; }
        public Guid? MasterPODDetentionTariffId { get; set; }




        //Relations
        public Company? Company { get; set; }
        public Job Job { get; set; }
        public Booking? Booking { get; set; }
        public PaymentType? PaymentType { get; set; }

        public Carrier? Carrier { get; set; }

        public Location? POR { get; set; }
        public Location? POL { get; set; }
        public Location? POD { get; set; }
        public Location? PODel { get; set; }
        public Location? TranshipmentPort { get; set; }

        public Incoterm? Incoterm { get; set; }

        public Location? FreightPaymentAt { get; set; }
        public PackageType? PackageType { get; set; }
        public LoadType? LoadType { get; set; }
        public MovementType? MovementType { get; set; }
        public BLReleaseType? BLReleaseType { get; set; }
        public BLStatus? BLStatus { get; set; }
        public ApplicationUser? SalesPerson { get; set; }
        public TransportType? TransportType { get; set; }
        public BLLevel? BLLevel { get; set; }
        public Person? IssuerParty { get; set; }
        public BLType? BLType { get; set; }
        public TradeType? TradeType { get; set; }
        public TariffBook? POLDetentionTariff { get; set; }
        public TariffBook? PODDetentionTariff { get; set; }

        public TariffBook? MasterPOLDetentionTariff { get; set; }
        public TariffBook? MasterPODDetentionTariff { get; set; }

        public List<BLContainer> BLContainers { get; set; } = new();
        public List<BLParty> BLParties { get; set; } = new();
        public List<CargoDetail> CargoDetails { get; set; } = new();
        public List<BLVoyage> BLVoyages { get; set; } = new();
        public List<BLFlight> BLFlights { get; set; } = new();
        public List<BLTrip> BLTrips { get; set; } = new();
        public List<BLRailJourney> BLRailJourneys { get; set; } = new();
        public List<BLCargoAllocation> CargoAllocations { get; set; } = new();
        public List<BLCharge> BLCharges { get; set; } = new();

        public void MarkAsPrinted()
        {
            LastPrintedAt = DateTime.Now;
        }

    }

    public class BLContainer : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int? ContainerSizeTypeId { get; set; }
        public int? LoadTypeId { get; set; }
        public int? Seq { get; set; }

        public int? FullMTId { get; set; }
        public int CntOwnTypeId { get; set; }

        public string? SealNumber { get; set; }
        public string? SealNumber2 { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }
        public decimal? TareWeight { get; set; }

        public decimal? BLContainerPercent { get; set; }
        public decimal? OverLength { get; set; }
        public decimal? OverWidth { get; set; }
        public decimal? OverHeight { get; set; }

        public int? DimensionUnitId { get; set; }
        public int TotalPackageCount { get; set; }
        public decimal? TotalVolumeCBM { get; set; }

        public string? ExportPermissionNo { get; set; }

        //Relations
        public BL? BL { get; set; }
        public ContainerUse? ContainerUse { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; } = null!;
        public MeasurementUnit? DimensionUnit { get; set; }
        public FullMT? FullMT { get; set; }
        public CntOwnType? CntOwnType { get; set; }
        public LoadType? LoadType { get; set; }
        public List<BLContainerCargoAllocation> CargoAllocations { get; set; } = new();
    }
    public class BLContainerCargoAllocation : BusinessEntity
    {
        public Guid BLContainerId { get; set; }
        public Guid CargoDetailId { get; set; }
        public decimal? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Volume { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public BLContainer BLContainer { get; set; } = null!;
        public CargoDetail CargoDetail { get; set; } = null!;
    }





    public class BLFlight : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid? AirlineId { get; set; }
        public Guid? FlightId { get; set; }
        public Guid? FlightDateId { get; set; }
        public string? FlightNo { get; set; } = string.Empty;
        public DateTime? FlightDateDate { get; set; }
        public string? FlightDateLocal { get; set; } = string.Empty;

        public string? DeclaredValueForCarriage { get; set; }
        public string? DeclaredValueForCustoms { get; set; }
        public string? HandlingInformation { get; set; }
        //Relations
        public BL? BL { get; set; }
        public Flight? Flight { get; set; }
        public FlightDate? FlightDate { get; set; }
        public Carrier? Airline { get; set; }
    }

    public class BLParty : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid? PartyId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ContactId { get; set; }
        public int RoleTypeId { get; set; }
        public string PartyTextInBL { get; set; } = string.Empty;
        public string? PartyNationalNo { get; set; } = string.Empty;
        public bool IsConsigneeToOrder { get; set; }
        public bool IsConsigneeToOrderOf { get; set; }
        public bool IsConsigneeToBearer { get; set; }
        public bool IsNotifySameAs { get; set; }

        public string? PartyTelephone { get; set; } = string.Empty;
        public string? PartyMobile { get; set; } = string.Empty;
        public string? PartyFax { get; set; } = string.Empty;
        public string? PartyEmail { get; set; } = string.Empty;

        //Relations
        public BL? BL { get; set; }
        public Person? Party { get; set; }
        public Branch? Branch { get; set; }
        public RoleType? PartyRoleType { get; set; }
        public PersonContact? Contact { get; set; }
    }
    public class BLVoyage : BusinessEntity
    {
        public Guid BLId { get; set; }
        public int VoyageTypeId { get; set; }
        public Guid? VoyageId { get; set; }
        public string VoyageNo { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public DateTime? ShippedOnBoardDate { get; set; }
        public DateTime? ReceivedForShipmentDate { get; set; }
        public bool IsFinal { get; set; }
        //Relations
        public BL? BL { get; set; }
        public Voyage? Voyage { get; set; }
        public VoyageType? VoyageType { get; set; }
    }






    public class BLTrip : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid? TripId { get; set; }
        public string? TruckNo { get; set; }
        public string? TrailerNo { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPassportNo { get; set; }
        public int? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public DateTime? LoadingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? CMRNo { get; set; }
        public string? TIRNo { get; set; }
        public string? ManifestNo { get; set; }
        //Relations
        public BL BL { get; set; }
        public Trip? Trip { get; set; }
        public List<BLContainerTripAllocation>? AllocatedContainers = new();
    }

    public class BLContainerTripAllocation : BusinessEntity
    {
        public Guid BLContainerId { get; set; }
        public Guid BLTripId { get; set; }

        public virtual BLContainer BLContainer { get; set; }
        public virtual BLTrip BLTrip { get; set; }

    }









    public class BLRailJourney : BusinessEntity
    {
        public Guid BLId { get; set; }

        public Guid? RailOperatorId { get; set; }
        public Guid? DepartureStationId { get; set; }
        public Guid? ArrivalStationId { get; set; }



        public string? TrainNo { get; set; }
        public string? WagonNo { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        //Relations
        public BL BL { get; set; }
        public Carrier? RailOperator { get; set; }
        public Location? DepartureStation { get; set; }
        public Location? ArrivalStation { get; set; }
    }

    public class BLCargoAllocation : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid CargoDetailId { get; set; }   //مربوط به بارنامه
        public Guid ShipmentCargoId { get; set; }  //مربوط به حمل واقعی

        public decimal PackageQty { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal ChargeableWeight { get; set; }
        public decimal Volume { get; set; }

        public string Remarks { get; set; } = string.Empty;

        //Relations
        public BL BL { get; set; }
        public CargoDetail CargoDetail { get; set; }
        public ShipmentCargo ShipmentCargo { get; set; }

    }

    public class BLCharge : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid BLId { get; set; }
        public int ChargeTypeId { get; set; }
        public int CalculationById { get; set; }
        public int? ContainerSizeTypeId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }   //Persisted ====> Quantity * UnitPrice [NotMapped]       public decimal TotalAmount => Quantity * UnitPrice;
        public int CurrencyId { get; set; }

        public int PaymentTypeId { get; set; }
        public int? PaymentCountryId { get; set; }
        public Guid? PaymentAgentId { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public Guid? BLOperationId { get; set; }

        //Relations
        public Company? Company { get; set; }
        public BL? BL { get; set; }
        public ChargeType? ChargeType { get; set; }
        public CalculationBy? CalculationBy { get; set; }
        public Currency? Currency { get; set; }
        public PaymentType? PaymentType { get; set; }
        public Country? PaymentCountry { get; set; }
        public Agent? PaymentAgent { get; set; }
        public BLOperation? BLOperation { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }

    }




}
