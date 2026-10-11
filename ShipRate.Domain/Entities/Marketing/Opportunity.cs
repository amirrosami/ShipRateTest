using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Marketing
{
    public class Opportunity : BusinessEntity
    {
        public int OpportunityNumber { get; set; }
        public string Title { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? LeadId { get; set; }
        public int? StageId { get; set; }
        public int TransportTypeId { get; set; }

        public decimal? EstimatedRevenue { get; set; }
        public int? CurrencyId { get; set; }

        public DateTime ExpectedCloseDate { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public string InternalNotes { get; set; }
        public Guid? WonJobId { get; set; }

        public int? LostReasonId { get; set; }
        public string LostReasonDescription { get; set; } = string.Empty;

        //Relations
        public Customer Customer { get; set; }
        public Lead Lead { get; set; }
        public JobStage Stage { get; set; }
        public TransportType TransportType { get; set; }
        public LostReason LostReason { get; set; }
        public ApplicationUser AssignedToUser { get; set; }
        public Job Job { get; set; }
    }
}
