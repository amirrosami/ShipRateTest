using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class ActivityOutcome : SystemEntity
    {
        public string ActivityOutcomeName { get; set; } = string.Empty;
        public string ActivityOutcomeLocalName { get; set; } = string.Empty;
    }
}
