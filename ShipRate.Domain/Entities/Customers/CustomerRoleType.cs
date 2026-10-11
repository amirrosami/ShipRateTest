using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerRoleType : BusinessEntity
    {
        public Guid CustomerId { get; set; }
        public int RoleTypeId { get; set; }

        //Relations
        public Customer Customer { get; set; }
        public RoleType RoleType { get; set; }
    }
}
