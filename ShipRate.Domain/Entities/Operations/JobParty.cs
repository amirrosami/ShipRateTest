using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobParty : BusinessEntity
    {
        public Guid JobId { get; set; }
        public int SequenceNo { get; set; }
        public Guid PartyId { get; set; }
        public Guid? BranchId { get; set; }
        public int RoleTypeId { get; set; }
        public bool IsPrimaryRole { get; set; }
        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        //Relations
        public Job Job { get; set; }
        public Person Party { get; set; }
        public RoleType RoleType { get; set; }
        public Branch Branch { get; set; }
    }
}
