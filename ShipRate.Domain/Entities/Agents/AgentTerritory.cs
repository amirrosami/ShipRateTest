using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentTerritory : BusinessEntity
    {
        public Guid AgentId { get; set; }
        public Guid TerritoryId { get; set; }

        //Relations
        public Agent Agent { get; set; }
        public Territory Territory { get; set; }
    }
}