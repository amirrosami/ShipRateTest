using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Marketing
{
    public class LeadTag : BusinessEntity
    {
        public Guid LeadId { get; set; }
        public int TagId { get; set; }


        //Relations
        public Lead Lead { get; set; }
        public Tag Tag { get; set; }
    }
}
