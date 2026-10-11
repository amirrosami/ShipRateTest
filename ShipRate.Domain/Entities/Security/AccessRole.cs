using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class AccessRole : BusinessEntity
    {
        public string AccessRoleName { get; set; }
    }
}
