using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentRoleCommissionPerSize : BusinessEntity
    {
        public Guid AgentRoleId { get; set; }
        public int ContainerSizeId { get; set; }
        public decimal AgentRoleCommissionPercent { get; set; }
        public decimal AgentRoleCommissionFixedValue { get; set; }
        public int CurrencyId { get; set; }

        //Relations
        public Currency? Currency { get; set; }
        public AgentRole AgentRole { get; set; }
        public ContainerSize ContainerSize { get; set; }
    }
}
