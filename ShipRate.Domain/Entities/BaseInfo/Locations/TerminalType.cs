using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Locations
{
    public class TerminalType : SystemEntity
    {
        public string TerminalTypeName { get; set; } = string.Empty;
    }
}
//Type(Container / Bulk / AirCargo)T