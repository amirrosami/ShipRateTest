using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class BankAccount : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string BankAccountTitle { get; set; } = string.Empty;
        public string BankAccountTitlePersian { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public Guid PersonId { get; set; }
        public int CurrencyId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BranchNo { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int BankAccountPurposeId { get; set; }


        //Relations
        public Company Company { get; set; }
        public Bank? Bank { get; set; }
        public Person? Person { get; set; }
        public Currency? Currency { get; set; }
        public BankAccountPurpose BankAccountPurpose { get; set; }
    }
}
