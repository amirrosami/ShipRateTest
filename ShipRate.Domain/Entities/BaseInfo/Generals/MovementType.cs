using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class MovementType : SystemEntity
    {
        public string MovementTypeCode { get; set; }
        public string MovementTypeDescription { get; set; }
        public int OriginNodeId { get; set; }
        public int DestinationNodeId { get; set; }
    }
}