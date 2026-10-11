using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.PettyCashControl
{
    public class PettyCash : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string Code { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal InitialAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? CustodianUserId { get; set; }

        // Relations
        public Company Company { get; set; }
        public Person Person { get; set; }
        public ApplicationUser CustodianUser { get; set; } = null!;
        public ICollection<PettyCashExpense> Expenses { get; set; } = new List<PettyCashExpense>();
        public ICollection<PettyCashRecharge> Recharges { get; set; } = new List<PettyCashRecharge>();
    }
}
