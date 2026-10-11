using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Marketing
{
    public class FollowUpTask : BusinessEntity
    {
        public Guid LeadId { get; private set; }
        public Guid? LeadActivityId { get; set; }
        public DateTime FollowUpTaskDueDate { get; set; }
        public int FollowUpStatusId { get; set; }
        public int FollowUpPriorityId { get; set; }
        public string FollowUpTaskDescription { get; set; } = string.Empty;
        public int ReminderChannelId { get; set; }

        //Relations
        public Lead Lead { get; set; }
        public LeadActivity LeadActivity { get; set; }
        public FollowUpStatus FollowUpStatus { get; set; }
        public FollowUpPriority FollowUpPriority { get; set; }
        public SendToolType ReminderChannel { get; set; }
    }
}
