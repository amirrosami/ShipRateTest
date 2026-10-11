using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Settings
{
    public class CompanyRole : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int CompanyRoleTypeId { get; set; }

        //Relations
        public Company Company { get; set; }
        public CompanyRoleType CompanyRoleType { get; set; }
    }
}
