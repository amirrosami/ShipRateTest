using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Customers
{
    public class SalesPersonHistory : BusinessEntity
    {
        public Guid CustomerId { get; set; }
        public Guid SalesPersonId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; } = string.Empty;

        //Relations
        public Customer Customer { get; set; }
        public Person SalesPerson { get; set; }
    }
}
