using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class CompanyRoleType : SystemEntity
    {
        public string CompanyRoleTypeName { get; set; } = string.Empty;
        public string CompanyRoleTypeLocalName { get; set; } = string.Empty;
        public int? RoleTypeId { get; set; }
    }
}
