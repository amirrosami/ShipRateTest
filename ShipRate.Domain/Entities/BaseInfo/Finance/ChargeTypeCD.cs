using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class ChargeTypeCreditDebit : BusinessEntity
    {
        public int ChargeTypeId { get; set; }
        public int CompanyId { get; set; }
        public int? Buy_Debit_AccTypeId { get; set; }
        public string? Buy_Debit_AccCode { get; set; } = string.Empty;
        public int? Buy_Credit_AccTypeId { get; set; }
        public string? Buy_Credit_AccCode { get; set; } = string.Empty;
        public int? Sell_Debit_AccTypeId { get; set; }
        public string? Sell_Debit_AccCode { get; set; } = string.Empty;
        public int? Sell_Credit_AccTypeId { get; set; }
        public string? Sell_Credit_AccCode { get; set; } = string.Empty;
        //Relations
        public ChargeType ChargeType { get; set; }
        public Company Company { get; set; }
        public CreditDebitAccType Buy_Debit_AccType { get; set; }
        public CreditDebitAccType Buy_Credit_AccType { get; set; }
        public CreditDebitAccType Sell_Debit_AccType { get; set; }
        public CreditDebitAccType Sell_Credit_AccType { get; set; }
    }

    public class CreditDebitAccType : SystemEntity
    {
        public string AccTypeName { get; set; }
    }

}
