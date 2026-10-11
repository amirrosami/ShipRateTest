using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRate : BusinessEntity
    {
        public Guid RateId { get; set; }
        public Guid CarrierId { get; set; }
        public Guid? AgentId { get; set; }
        //Relations
        public Rate Rate { get; set; }
        public Carrier Carrier { get; set; }
        public Agent Agent { get; set; }
    }
}
