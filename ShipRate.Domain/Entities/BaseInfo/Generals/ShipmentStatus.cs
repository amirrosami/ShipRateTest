using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class ShipmentStatus : SystemEntity
    {
        public string ShipmentStatusName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool isFinal { get; set; }
    }
    public class ShipmentLegStatus : SystemEntity
    {
        public string ShipmentLegStatusName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool isFinal { get; set; }
    }

}
