using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Customers;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractCustomer : BusinessEntity
    {
        public Guid ContractCustomerId { get; set; }
        public Guid ContractId { get; set; }
        public int CustomerRoleTypeId { get; set; }
        public Guid CustomerId { get; set; }
        //Relations
        public Contract? Contract { get; set; }
        public RoleType? CustomerRoleType { get; set; }
        public Customer? Customer { get; set; }
    }
}
