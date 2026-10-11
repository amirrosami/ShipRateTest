using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentActScope : BusinessEntity
    {
        public Guid AgentId { get; set; }
        public int AgentScopeId { get; set; }

        //Relations
        public Agent Agent { get; set; }
        public AgentScope AgentScope { get; set; }
    }
}
