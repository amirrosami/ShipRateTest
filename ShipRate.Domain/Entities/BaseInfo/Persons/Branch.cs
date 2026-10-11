using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class Branch : BusinessEntity
    {
        public Guid ParentId { get; set; }
        public int BranchTypeId { get; set; }
        public string? BranchCode { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Guid? LocationId { get; set; }
        public string? ManagerName { get; set; }
        public string? Address { get; set; }
        public string? Remarks { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        //Relations
        public Person Parent { get; set; }
        public Location Location { get; set; }
        public Country Country { get; set; }
        public BranchType BranchType { get; set; }
    }
}
