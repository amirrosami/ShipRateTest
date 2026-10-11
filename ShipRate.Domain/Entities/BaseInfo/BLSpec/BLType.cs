using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.BaseInfo.BLSpec
{
    public class BLType : SystemEntity
    {
        public string BLTypeCode { get; set; } = string.Empty;
        public string BLTypeName { get; set; } = string.Empty;
        public int? TransportTypeId { get; set; }

        //Relations 
        public TransportType TransportType { get; set; }
    }
}
