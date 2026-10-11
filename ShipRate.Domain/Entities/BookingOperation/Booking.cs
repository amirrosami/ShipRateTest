using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BookingSpec;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Domain.Entities.Settings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class Booking : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int CompanyRoleTypeId { get; set; }

        public int LoadTypeId { get; set; }
        public int ServiceTypeId { get; set; }
        public int TransportTypeId { get; set; }

        public Guid JobId { get; set; }
        public string BookingNo { get; set; } = string.Empty;
        public string? LineBookingNo { get; set; }

        public Guid? RateRequestId { get; set; }
        public Guid? BuyQuoteId { get; set; }
        public Guid? ContractId { get; set; }


        public Guid? CarrierPartyId { get; set; }
        public Guid? VendorPartyId { get; set; }
        public Guid? BookingAgentPartyId { get; set; }


        //Common For All Transport Types
        public Guid? OriginId { get; set; }
        public Guid? DestId { get; set; }
        public Guid? TranshipPortId { get; set; }
        public Guid? POLId { get; set; }
        public Guid? PODId { get; set; }

        public DateTime? CargoReadyDate { get; set; }
        public DateTime? CutOffDate { get; set; }
        public DateTime? SICutOffDate { get; set; }
        public DateTime? VGMCutOffDate { get; set; }
        public DateTime? DocumentCutOffDate { get; set; }



        public Guid? ParentBookingId { get; set; }
        public int BookingStatusId { get; set; }
        public string? Remarks { get; set; }


        //Relations
        public Job? Job { get; set; }
        public TransportType TransportType { get; set; }
        public Contract.Contract? Contract { get; set; }
        public CompanyRoleType CompanyRoleType { get; set; }
        public Booking? ParentBooking { get; set; }
        public RateRequest? RateRequest { get; set; }
        public BuyQuote? BuyQuote { get; set; }
        public Carrier? Carrier { get; set; }
        public Person? VendorParty { get; set; }
        public Person? BookingAgentParty { get; set; }

        public Location? Origin { get; set; }
        public Location? POL { get; set; }
        public Location? POD { get; set; }
        public Location? Dest { get; set; }
        public Location? TranshipPort { get; set; }
        public Company Company { get; set; }
        public BookingStatus BookingStatus { get; set; }

        public LoadType LoadType { get; set; }
        public ServiceType ServiceType { get; set; }



        public virtual BookingSeaDetail? SeaDetail { get; set; }
        public virtual ICollection<BookingRoadDetail> RoadDetails { get; set; } = new HashSet<BookingRoadDetail>();
        public virtual ICollection<BookingRailDetail> RailDetails { get; set; } = new HashSet<BookingRailDetail>();
        public virtual BookingAirDetail? AirDetail { get; set; }

        public ICollection<BookingLeg> BookingLegs { get; set; } = new HashSet<BookingLeg>();
        public ICollection<BookingContainerType> BookingContainerTypes { get; set; } = new HashSet<BookingContainerType>();
        public ICollection<BookingParty> BookingParties { get; set; } = new HashSet<BookingParty>();
        public virtual ICollection<CargoDetail> CargoDetails { get; set; } = new HashSet<CargoDetail>();
        public virtual ICollection<Charge> Charges { get; set; } = new HashSet<Charge>();
    }

    public class BookingLeg : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public int SequenceNo { get; set; }
        public int TransportTypeId { get; set; }

        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }

        public DateTime? PlannedETD { get; set; }
        public DateTime? PlannedETA { get; set; }

        [MaxLength(400)]
        public string? Remarks { get; set; }

        //Relations
        public Booking Booking { get; set; } = null!;
        public ShipmentLeg ShipmentLeg { get; set; } = null!;
        public TransportType TransportType { get; set; } = null!;
        public Location? FromLocation { get; set; }
        public Location? ToLocation { get; set; }

        public virtual ICollection<CargoDetail> CargoDetails { get; set; } = new HashSet<CargoDetail>();
        public virtual ICollection<Charge> Charges { get; set; } = new HashSet<Charge>();
    }



    [Table("BookingSeaDetails", Schema = "ops")]
    public class BookingSeaDetail : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        //public Guid? POLId { get; set; } // Port of Loading
        //public Guid? PODId { get; set; } // Port of Discharge
        //public Guid? FinalDestinationPortId { get; set; }

        public Guid? ShippingLineId { get; set; }

        [StringLength(100)]
        public string? ServiceName { get; set; }

        //[StringLength(50)]
        //public string? LineBookingNo { get; set; }

        //public int? BLTypeId { get; set; }
        //public int? FreightTermId { get; set; }


        public DateTime? CYCutOffDate { get; set; }
        public DateTime? SICutOffDate { get; set; }
        public DateTime? DOCutOffDate { get; set; }
        public DateTime? VGMCutOffDate { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        // Container / movement related
        public Guid? EmptyPickupDepotId { get; set; }
        public Guid? EmptyReturnDepotId { get; set; }
        public Guid? ReceivingTerminalId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }


        //Relations
        public virtual ICollection<BookingSeaVoyage> Voyages { get; set; } = new HashSet<BookingSeaVoyage>();

        //public Location? POL { get; set; }
        //public Location? POD { get; set; }
        //public Location? FinalDestinationPort { get; set; }

        public Carrier? ShippingLine { get; set; }
        public Location? EmptyPickupDepot { get; set; }
        public Location? EmptyReturnDepot { get; set; }
        public Location? ReceivingTerminal { get; set; }

    }


    [Table("BookingSeaVoyages", Schema = "ops")]
    public class BookingSeaVoyage : BusinessEntity
    {
        public Guid BookingSeaDetailId { get; set; }
        public virtual BookingSeaDetail BookingSeaDetail { get; set; } = null!;
        public Guid? BookingLegId { get; set; } // Optional link to Booking leg

        public int SequenceNo { get; set; }

        public Guid? FromPortId { get; set; }
        public Guid? ToPortId { get; set; }

        public Guid? VoyageId { get; set; }

        public Guid? VesselId { get; set; }

        [StringLength(150)]
        public string? VesselName { get; set; }

        [StringLength(50)]
        public string? VoyageNo { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }

        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }

        public bool IsTransshipment { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        //Relations
        public Location? FromPort { get; set; }
        public Location? ToPort { get; set; }
        public Vessel? Vessel { get; set; }
        public BookingLeg? BookingLeg { get; set; }
        public Voyage? Voyage { get; set; }


    }

    [Table("BookingRoadDetails", Schema = "ops")]
    public class BookingRoadDetail : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        public Guid? BookingLegId { get; set; } // Optional link to Booking leg

        public Guid? PickupLocationId { get; set; }
        public Guid? DeliveryLocationId { get; set; }

        [StringLength(500)]
        public string? PickupAddress { get; set; }

        [StringLength(500)]
        public string? DeliveryAddress { get; set; }

        public int? TruckTypeId { get; set; }
        public int? TruckCount { get; set; }

        [StringLength(150)]
        public string? DriverName { get; set; }

        [StringLength(50)]
        public string? DriverMobile { get; set; }

        [StringLength(50)]
        public string? TruckPlateNo { get; set; }

        public Guid? ContainerPickupDepotId { get; set; }
        public Guid? ContainerReturnDepotId { get; set; }

        public DateTime? PlannedLoadingDate { get; set; }
        public DateTime? PlannedUnloadingDate { get; set; }

        [StringLength(100)]
        public string? RoadPermitNo { get; set; }

        [StringLength(100)]
        public string? CMRNo { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }


        public virtual BookingLeg? BookingLeg { get; set; }
        public virtual Location? PickupLocation { get; set; }
        public virtual Location? DeliveryLocation { get; set; }
        public TruckType? TruckType { get; set; }
        public Location? ContainerPickupDepot { get; set; }
        public Location? ContainerReturnDepot { get; set; }

    }


    [Table("BookingRailDetails", Schema = "ops")]
    public class BookingRailDetail : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        public Guid? BookingLegId { get; set; } // Optional link to Booking leg

        public Guid? OriginStationId { get; set; }
        public Guid? DestinationStationId { get; set; }

        public Guid? RailOperatorId { get; set; }

        [StringLength(50)]
        public string? TrainNo { get; set; }

        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }

        [StringLength(100)]
        public string? RailWaybillNo { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }


        public virtual BookingLeg? BookingLeg { get; set; }
        public virtual Location? OriginStation { get; set; }
        public virtual Location? DestinationStation { get; set; }
        public Person? RailOperator { get; set; }

    }


    [Table("BookingAirDetails", Schema = "ops")]
    public class BookingAirDetail : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;

        public Guid? DepartureAirportId { get; set; }
        public Guid? ArrivalAirportId { get; set; }

        public Guid? AirlineId { get; set; }

        public string? AirlineBookingNo { get; set; }

        [StringLength(50)]
        public string? MAWBNo { get; set; } // Master Air Waybill

        [StringLength(50)]
        public string? HAWBNo { get; set; } // House Air Waybill

        public DateTime? AirCutOffDate { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public Guid? CargoTerminalId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public virtual ICollection<BookingAirFlight> Flights { get; set; } = new HashSet<BookingAirFlight>();
        public Location? DepartureAirport { get; set; }
        public Location? ArrivalAirport { get; set; }
        public Carrier? Airline { get; set; }
        public Location? CargoTerminal { get; set; }

    }

    [Table("BookingAirFlights", Schema = "ops")]
    public class BookingAirFlight : BusinessEntity
    {
        public Guid BookingAirDetailId { get; set; }

        public int SequenceNo { get; set; }

        public Guid? FromAirportId { get; set; }
        public Guid? ToAirportId { get; set; }

        public Guid? AirlineId { get; set; }

        [StringLength(50)]
        public string? FlightNo { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }

        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }


        //Relations
        public Location? FromAirport { get; set; }
        public Location? ToAirport { get; set; }
        public Carrier? Airline { get; set; }
        public virtual BookingAirDetail BookingAirDetail { get; set; } = null!;

    }

    [Table("BookingParties", Schema = "ops")]
    public class BookingParty : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public int PartyRoleTypeId { get; set; }
        public Guid PartyId { get; set; }
        //Relations
        public Booking? Booking { get; set; }
        public RoleType? PartyRoleType { get; set; }
        public Person? Party { get; set; }
    }

    [Table("BookingContainerTypes", Schema = "ops")]
    public class BookingContainerType : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public int CntOwnTypeId { get; set; }
        public int ContainerCount { get; set; }


        // OverDimension
        public decimal? OverLengthCM { get; set; }
        public decimal? OverWidthCM { get; set; }
        public decimal? OverHeightCM { get; set; }

        //Relations
        public Booking Booking { get; set; } = null!;
        public ContainerSizeType? ContainerSizeType { get; set; }
        public CntOwnType? CntOwnType { get; set; }
    }
}
