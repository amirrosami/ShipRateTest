using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.BaseInfo.Vehicles
{
    public class Vessel : BusinessEntity
    {
        public int? CompanyId { get; set; } //مالک یا عامل
        public int VesselTypeId { get; set; }
        public string VesselCode { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string CallSign { get; set; } = string.Empty;

        public string IMONumber { get; set; } = string.Empty;
        public string MMSI { get; set; } = string.Empty;   //Maritime Mobile Service Identity



        public long DWT { get; set; }

        public long? GrossTonnage { get; set; }
        public long? NetTonnage { get; set; }

        public int? BuiltYear { get; set; }
        public int? CapacityTEU { get; set; }
        public int? FlagCountryId { get; set; }
        public Guid? OwnerId { get; set; }    //Owner
        public Guid? ShippingLineId { get; set; }    //Operator
        public int? OwnTypeId { get; set; }

        //Relations
        public VesselType? VesselType { get; set; }
        public Carrier? ShippingLine { get; set; }
        public Country? FlagCountry { get; set; }
        public VesselOwnType? OwnType { get; set; }
        public Person? Owner { get; set; }

        //public List<Voyage>? Voyages { get; set; }

        //public override string Validate()
        //{
        //    return base.Validate();
        //}
    }
}

/*Bishtar Barrasi Shavad
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
