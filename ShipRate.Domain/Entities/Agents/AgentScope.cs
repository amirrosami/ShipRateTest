using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentScope : SystemEntity
    {
        public string AgentScopeCode { get; set; }
        public string AgentScopeName { get; set; }
    }
}
