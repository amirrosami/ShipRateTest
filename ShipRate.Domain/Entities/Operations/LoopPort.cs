using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Operations
{
    public class LoopPort : BusinessEntity
    {
        public Guid LoopId { get; set; }
        public int SequenceNo { get; set; }
        public Guid PortId { get; set; }
        public bool isLoadPort { get; set; }
        public bool isDischargePort { get; set; }
        public bool isTranshipPort { get; set; }
        public string LoopPortRemarks { get; set; } = string.Empty;

        //Relations
        public Loop? Loop { get; set; }
        public Location? Port { get; set; }
    }
}
