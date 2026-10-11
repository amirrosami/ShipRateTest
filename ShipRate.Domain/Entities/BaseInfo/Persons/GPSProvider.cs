using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class GPSProvider : BusinessEntity
    {
        public string SiteAddress { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Person Person { get; set; }
    }
}
