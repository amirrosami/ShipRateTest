using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgencyContract : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string ContractNumber { get; set; }
        public string AgreementType { get; set; } //AgreementType VARCHAR(30) NOT NULL,              -- LINER_AGENT, NVOCC_AGENT, BOOKING_AGENT, GENERAL_AGENT, PORT_AGENT
        public Guid AgentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float AgencyContractBCMPercent { get; set; }
        public string Terms { get; set; }
        //Relations
        public Agent? Agent { get; set; }
        public Company? Company { get; set; }
    }
}
