using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.PettyCashControl
{
    public class ExpenseCategory : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string Title { get; set; } = null!;
        public decimal MaxAllowedAmount { get; set; }
        public string AccountCode { get; set; }
        //Relations
        public Company Company { get; set; }
    }
}
