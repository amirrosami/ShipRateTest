using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.PettyCashControl
{
    public class CostCenter : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;

        //Relations
        public Company Company { get; set; }
        //public ICollection<PettyCashExpense> Expenses { get; set; } = new List<PettyCashExpense>();
    }
}