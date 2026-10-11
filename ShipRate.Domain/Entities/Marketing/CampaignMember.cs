using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Marketting;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Marketing
{
    public class CampaignMember : BusinessEntity
    {
        public Guid CampaignId { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? ContactId { get; set; }
        public int MemberStatusId { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public bool? IsConverted { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Campaign Campaign { get; set; }
        public Lead Lead { get; set; }
        public Person Person { get; set; }
        public PersonContact Contact { get; set; }
        public MemberStatus MemberStatus { get; set; }

    }
}
