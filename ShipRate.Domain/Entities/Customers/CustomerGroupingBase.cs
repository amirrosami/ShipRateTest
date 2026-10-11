using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerGroupingBase : BusinessEntity
    {
        public string CustomerGroupingBaseName { get; set; } = string.Empty;
        //public string CustomerGroupingBaseLocalName { get; set; } = string.Empty;



        //Relations
        public List<CustomerGroup> CustomerGroups { get; set; }
    }
}
