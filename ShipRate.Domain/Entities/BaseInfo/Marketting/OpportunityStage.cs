using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class OpportunityStage : SystemEntity
    {
        public string OpportunityStageName { get; set; }
        public string OpportunityStageLocalName { get; set; }
        public int WinProbability { get; set; }
    }
}
