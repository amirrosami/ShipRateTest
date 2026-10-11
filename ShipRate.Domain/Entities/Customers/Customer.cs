using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Customers
{
    public class Customer : BusinessEntity
    {
        public Guid? IndustryId { get; set; }
        public Guid? SalesUserId { get; set; }
        public string Remarks { get; set; } = string.Empty;

        //Relations 
        public Person Person { get; set; }
        public ApplicationUser SalesUser { get; set; }
        public Industry? Industry { get; set; }
        public List<CustomerTradeType> CustomerTradeTypes { get; set; }
    }
}