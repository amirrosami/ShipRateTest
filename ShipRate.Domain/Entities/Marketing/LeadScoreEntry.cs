using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Enums;
namespace ShipRate.Domain.Entities.Marketing
{
    public class LeadScoreEntry : BusinessEntity
    {
        public int Score { get; }
        public EnumLeadScoreReason LeadScoreReasonId { get; }
    }
}
