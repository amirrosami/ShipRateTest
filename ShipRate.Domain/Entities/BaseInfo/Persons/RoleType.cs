using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class RoleType : SystemEntity
    {
        public string RoleTypeName { get; set; } = string.Empty;
        public int ObjectTypeId { get; set; }
    }
}


//ObjectTypeId ex: Customer,Agent