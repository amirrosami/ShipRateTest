using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentLeg : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public int SequenceNo { get; set; }
        public int? TransportTypeId { get; set; }
        public int? LegTypeId { get; set; }
        public Guid FromLocationId { get; set; }
        public Guid ToLocationId { get; set; }
        public Guid? CarrierId { get; set; }
        //public Guid? TransportMovementId { get; set; }

        public Guid? VoyageId { get; set; }
        public Guid? VesselId { get; set; }

        public Guid? FlightId { get; set; }
        public Guid? FlightDateId { get; set; }

        public Guid? TripId { get; set; }
        public Guid? TruckId { get; set; }
        public Guid? DriverId { get; set; }

        public string? ReceiptPlaceText { get; set; }
        public string? DeliveryPlaceText { get; set; }





        public DateTime? PlannedDepartureDate { get; set; }
        public DateTime? PlannedArrivalDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }


        public int? TotalPackages { get; set; }
        public int? PackageTypeId { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? ChargeableWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }




        public string? Remarks { get; set; }
        public int? LegStatusId { get; set; }



        //Relations
        public Shipment Shipment { get; set; }
        public LegType LegType { get; set; }
        //public TransportMovement TransportMovement { get; set; }
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public Carrier? Carrier { get; set; }
        public TransportType TransportType { get; set; }
        public Vessel Vessel { get; set; }
        public Voyage Voyage { get; set; }
        public Flight Flight { get; set; }
        public FlightDate FlightDate { get; set; }
        public Trip Trip { get; set; }
        public Truck Truck { get; set; }
        public Driver Driver { get; set; }
        public ShipmentLegStatus LegStatus { get; set; }

    }
}
