using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerFinanceSetting : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal CreditLimit { get; set; }
        public int CurrencyId { get; set; }
        public int? RiskLevelId { get; set; }
        public string TaxNumber { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public string FinanceRemarks { get; set; } = string.Empty;

        //Rel
        public Company Company { get; set; }
        public Currency Currency { get; set; }
        public Customer Customer { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }
}
