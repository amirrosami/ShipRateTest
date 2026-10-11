using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class TeamType : SystemEntity
    {
        public string TeamTypeName { get; set; }
        public string TeamTypeLocalName { get; set; }
    }
}
