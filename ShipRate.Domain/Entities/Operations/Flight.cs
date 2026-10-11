using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Flight : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public Guid AirLineId { get; set; }
        public Guid OriginAirportId { get; set; }
        public Guid DestinationAirportId { get; set; }

        //Relations
        public Company Company { get; set; }
        public Location OriginAirPort { get; set; }
        public Location DestinationAirPort { get; set; }
        public Carrier AirLine { get; set; }
    }
}
