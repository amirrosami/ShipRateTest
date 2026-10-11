using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Settings
{
    public class ContextType : SystemEntity
    {
        public string ContextTypeName { get; set; } = string.Empty;
    }
}


/*
 * FORWARDING
NVOCC
SHIPPING_LINE
LINE_AGENT
CUSTOMS_BROKER
TRANSIT_CARRIER*/
