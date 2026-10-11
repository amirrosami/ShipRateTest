//این احتمالا دست کاری شود و حذف شود 
using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Agents
{
    public class AgentType : SystemEntity
    {
        public string AgentTypeName { get; set; } = string.Empty;
        public bool CanIssueContract { get; set; }
        public bool CanIssueBooking { get; set; }
        public bool CanIssueBL { get; set; }
        public bool CanIssueFP { get; set; }
        //Relations
    }
}