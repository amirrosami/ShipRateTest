using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerGroup : BusinessEntity
    {
        public Guid CustomerGroupingBaseId { get; set; }
        public string CustomerGroupName { get; set; } = string.Empty;


        //Relations
        public CustomerGroupingBase CustomerGroupingBase { get; set; }
        public List<CustomerBelongsToGroup> CustomerBelongsToGroups { get; set; }
    }
}