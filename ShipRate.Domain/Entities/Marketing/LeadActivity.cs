using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Marketting;

namespace ShipRate.Domain.Entities.Marketing
{
    public class LeadActivity : BusinessEntity
    {
        public Guid LeadId { get; set; }
        public int? ActivityTypeId { get; set; }
        public int? ActivityResultId { get; set; }
        public int? ActivityOutcomeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime? FollowUpDate { get; set; }

        //Relations
        public Lead Lead { get; set; }
        public ActivityType ActivityType { get; set; }
        public ActivityResult ActivityResult { get; set; }
        public ActivityOutcome ActivityOutcome { get; set; }


    }
}
