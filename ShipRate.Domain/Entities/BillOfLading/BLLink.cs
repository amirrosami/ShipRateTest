using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLLink : BusinessEntity
    {
        public Guid FromBLId { get; set; }
        public Guid ToBLId { get; set; }
        public int BLLinkTypeId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public BL FromBL { get; set; }
        public BL ToBL { get; set; }
        public BLLinkType BLLinkType { get; set; }
    }
}
