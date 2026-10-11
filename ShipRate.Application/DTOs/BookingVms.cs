using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{

    public class BookingChargeVm
    {
        public Guid? Id { get; set; }

        public int ChargeCodeId { get; set; }
        public int ChargeSideId { get; set; }
        public Guid PartyId { get; set; }
        public Guid PartyName { get; set; }
        public int CurrencyId { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        public bool IsTaxable { get; set; }
        public decimal? TaxPercent { get; set; }
        public string? Remarks { get; set; }



        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> ChargeSides { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();


    }



    public class BookingFormVm
    {
        // Primary Keys & Parent References
        public Guid? Id { get; set; }
        public Guid? JobId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyRoleTypeId { get; set; }

        // Core Properties
        [StringLength(100)]
        public string BookingNo { get; set; } = string.Empty;
        public string? LineBookingNo { get; set; }

        public int LoadTypeId { get; set; }
        public int ServiceTypeId { get; set; }
        public int TransportTypeId { get; set; } // Sea, Road, Rail, Air

        public Guid? RateRequestId { get; set; }
        public Guid? BuyQuoteId { get; set; }
        public Guid? ContractId { get; set; }

        // Primary Parties
        public Guid? CarrierPartyId { get; set; }
        public string? CarrierPartyName { get; set; }

        public Guid? VendorPartyId { get; set; }
        public string? VendorPartyName { get; set; }

        public Guid? BookingAgentPartyId { get; set; }
        public string? BookingAgentPartyName { get; set; }


        // Common Locations
        public Guid? OriginId { get; set; }
        public Guid? DestId { get; set; }
        public Guid? TranshipPortId { get; set; }
        public Guid? POLId { get; set; }
        public Guid? PODId { get; set; }


        // Common Locations
        public string? OriginName { get; set; }
        public string? DestName { get; set; }
        public string? TranshipPortName { get; set; }
        public string? POLName { get; set; }
        public string? PODName { get; set; }




        // Common Dates
        public DateTime? CargoReadyDate { get; set; }
        public DateTime? CutOffDate { get; set; }
        public DateTime? SICutOffDate { get; set; }
        public DateTime? VGMCutOffDate { get; set; }
        public DateTime? DocumentCutOffDate { get; set; }

        public Guid? ParentBookingId { get; set; }
        public int BookingStatusId { get; set; }
        public string? Remarks { get; set; }

        // Mode Specific Details (Only active model will be mapped/populated based on TransportTypeId)
        public BookingSeaDetailVm SeaDetail { get; set; } = new();
        public List<BookingRoadDetailVm> RoadDetails { get; set; } = new();
        public List<BookingRailDetailVm> RailDetails { get; set; } = new();
        public BookingAirDetailVm AirDetail { get; set; } = new();

        // Collections
        public List<BookingLegVm> BookingLegs { get; set; } = new();
        public List<BookingContainerTypeVm> BookingContainerTypes { get; set; } = new();
        public List<BookingPartyVm> BookingParties { get; set; } = new();
        //public List<CargoItemVm> CargoItems { get; set; } = new();
        public List<CargoDetailListItemVm> CargoItems { get; set; } = new();
        public List<BookingChargeVm> Charges { get; set; } = new();
    }

    public class BookingSeaDetailVm
    {
        public Guid? Id { get; set; }
        public Guid? POLId { get; set; }
        public Guid? PODId { get; set; }
        public Guid? FinalDestinationPortId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public string? ServiceName { get; set; }
        public string? LineBookingNo { get; set; }
        public int? BLTypeId { get; set; }
        public int? FreightTermId { get; set; }
        public DateTime? CYCutOffDate { get; set; }
        public DateTime? SICutOffDate { get; set; }
        public DateTime? DOCutOffDate { get; set; }
        public DateTime? VGMCutOffDate { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public Guid? EmptyPickupDepotId { get; set; }
        public Guid? EmptyReturnDepotId { get; set; }
        public Guid? ReceivingTerminalId { get; set; }
        public string? Remarks { get; set; }
        public List<BookingSeaVoyageVm> Voyages { get; set; } = new();
    }

    public class BookingSeaVoyageVm
    {
        public Guid? Id { get; set; }
        public int SequenceNo { get; set; }

        public Guid? FromPortId { get; set; }
        public Guid? ToPortId { get; set; }

        public string? FromPortName { get; set; }
        public string? ToPortName { get; set; }


        public Guid? VoyageId { get; set; }
        public Guid? VesselId { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public bool IsTransshipment { get; set; }
        public string? Remarks { get; set; }
    }

    public class BookingRoadDetailVm
    {
        public int Index { get; set; }

        public Guid? Id { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? PickupLocationId { get; set; }
        public Guid? DeliveryLocationId { get; set; }

        public string? PickupLocationName { get; set; }
        public Guid? DeliveryLocationName { get; set; }

        public string? PickupAddress { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? TruckTypeId { get; set; }
        public int? TruckCount { get; set; }
        public string? DriverName { get; set; }
        public string? DriverMobile { get; set; }
        public string? TruckPlateNo { get; set; }
        public Guid? ContainerPickupDepotId { get; set; }
        public Guid? ContainerReturnDepotId { get; set; }
        public DateTime? PlannedLoadingDate { get; set; }
        public DateTime? PlannedUnloadingDate { get; set; }
        public string? RoadPermitNo { get; set; }
        public string? CMRNo { get; set; }
        public string? Remarks { get; set; }
    }

    public class BookingRailDetailVm
    {
        public Guid? Id { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? OriginStationId { get; set; }
        public Guid? DestinationStationId { get; set; }
        public Guid? RailOperatorId { get; set; }
        public string? TrainNo { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string? RailWaybillNo { get; set; }
        public string? Remarks { get; set; }
    }

    public class BookingAirDetailVm
    {
        public Guid? Id { get; set; }
        public Guid? DepartureAirportId { get; set; }
        public Guid? ArrivalAirportId { get; set; }
        public Guid? AirlineId { get; set; }
        public string? AirlineBookingNo { get; set; }
        public string? MAWBNo { get; set; }
        public string? HAWBNo { get; set; }
        public DateTime? AirCutOffDate { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public Guid? CargoTerminalId { get; set; }
        public string? Remarks { get; set; }
        public List<BookingAirFlightVm> Flights { get; set; } = new();
    }

    public class BookingAirFlightVm
    {
        public Guid? Id { get; set; }
        public int SequenceNo { get; set; }
        public Guid? FromAirportId { get; set; }
        public Guid? ToAirportId { get; set; }
        public Guid? AirlineId { get; set; }
        public string? FlightNo { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public string? Remarks { get; set; }
    }

    public class BookingLegVm
    {
        public Guid? Id { get; set; }
        public Guid? ShipmentLegId { get; set; }
        public int SequenceNo { get; set; }
        public int TransportTypeId { get; set; }
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }

        public string? FromLocationName { get; set; }
        public string? ToLocationName { get; set; }

        public DateTime? PlannedETD { get; set; }
        public DateTime? PlannedETA { get; set; }
        public string? Remarks { get; set; }
    }

    public class BookingContainerTypeVm
    {
        public Guid? Id { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public int CntOwnTypeId { get; set; }
        public int ContainerCount { get; set; }
        public decimal? OverLengthCM { get; set; }
        public decimal? OverWidthCM { get; set; }
        public decimal? OverHeightCM { get; set; }
    }

    public class BookingPartyVm
    {
        public Guid? Id { get; set; }
        public int PartyRoleTypeId { get; set; }
        public Guid PartyId { get; set; }
    }
}
