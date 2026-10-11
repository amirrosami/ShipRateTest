//بعدا با TransitShipment چک شود حتما
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Trip : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid CarrierId { get; set; }
        public Guid TruckId { get; set; }
        public string TripNumber { get; set; }
        public Guid DriverId { get; set; }
        public Guid FromLocationId { get; set; }
        public Guid ToLocationId { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public int TripStatusId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Company Company { get; set; }
        //public TransportMovement TransportMovement { get; set; }
        public Driver Driver { get; set; }
        public Truck Truck { get; set; }
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public TripStatus TripStatus { get; set; }
        public Carrier Carrier { get; set; }
    }
}


/*
 *Trip Status dideh shavd 
 * 
 * 
 * 
 * 
 */
