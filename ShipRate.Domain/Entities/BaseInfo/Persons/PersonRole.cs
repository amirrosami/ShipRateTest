using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonRole : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public int RoleTypeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        //Relations
        public Person Person { get; set; }
        public RoleType RoleType { get; set; }
    }
}
