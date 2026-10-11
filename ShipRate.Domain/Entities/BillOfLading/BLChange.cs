using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLChange : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int BLChangeTypeId { get; set; }
        public Guid NewBLId { get; set; }
        public Guid OldBLId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public BLChangeType? BLChangeType { get; set; }
        public BL? NewBL { get; set; }
        public BL? OldBL { get; set; }
        public Company? Company { get; set; }
    }
}