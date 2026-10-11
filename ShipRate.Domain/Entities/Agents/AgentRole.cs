using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentRole : BusinessEntity
    {
        public Guid AgentId { get; set; }
        public Guid CarrierId { get; set; }
        public int AgentRoleTypeId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public decimal CommissionRatePercent { get; set; }
        public bool IsExclusive { get; set; }
        //Relations
        public Agent Agent { get; set; }
        public AgentRoleType AgentRoleType { get; set; }
        public Carrier Carrier { get; set; }
    }
}
