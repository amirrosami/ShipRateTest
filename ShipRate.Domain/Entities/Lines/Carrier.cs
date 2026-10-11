using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Lines
{
    public class Carrier : BusinessEntity
    {
        public string CarrierCode { get; set; } = string.Empty;
        public int CarrierTypeId { get; set; } // ShippingLine, NVOCC, Air, Road, Rail

        //AirLine Specific
        public string AWBPrefix { get; set; }


        //Shipping Line Specific
        public bool IsVesselOwner { get; set; }
        public string BookingEmail { get; set; }

        // Road / Rail specific
        public string? RegistrationNumber { get; set; } // Optional: License
        public string? OperatingRegion { get; set; }

        public string TrackingUrl { get; set; } = string.Empty;
        //Relation
        public CarrierType CarrierType { get; set; }
        public Person Person { get; set; }
        public List<ShippingLineService>? ShippingLineServices { get; set; }
    }
}
