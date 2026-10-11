using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerBelongsToGroup : BusinessEntity
    {
        public Guid CustomerGroupId { get; set; }
        public Guid CustomerId { get; set; }

        public DateTime EnterDate { get; set; }
        public DateTime ExitDate { get; set; }

        public string EnterRemarks { get; set; } = string.Empty;
        public string ExitRemarks { get; set; } = string.Empty;

        //Relations
        public CustomerGroup? CustomerGroup { get; set; }
        public Customer? Customer { get; set; }
    }
}
