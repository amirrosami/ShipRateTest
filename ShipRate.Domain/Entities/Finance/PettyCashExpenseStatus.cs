using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class PettyCashExpenseStatus : SystemEntity
    {
        public string StatusName { get; set; }
        public string? StatusLocalName { get; set; }
    }

    public class PettyCashRechargeStatus : SystemEntity
    {
        public string StatusName { get; set; }
        public string? StatusLocalName { get; set; }
    }



}
