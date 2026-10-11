using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Rate
{
    public class RateCountryPort : BusinessEntity
    {
        public Guid RateId { get; set; }
        public int? CountryId { get; set; }
        public Guid? PortId { get; set; }
        public int DischargeLoadId { get; set; } //Discharge=Destination,Load=Origin
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Rate? Rate { get; set; }
        public DischargeLoad DischargeLoad { get; set; }
        public Country Country { get; set; }
        public Location Port { get; set; }
    }
}
