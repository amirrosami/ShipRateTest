using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class IPGTerminal : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string IPGTerminalId { get; set; }
        public string IPGTerminalName { get; set; }
        //Relations
        public Company Company { get; set; }
        public ICollection<IPGTerminalBankAccount> IPGAccounts { get; set; } = new HashSet<IPGTerminalBankAccount>();
    }
    public class IPGTerminalBankAccount : BusinessEntity
    {
        public Guid IPGTerminalId { get; set; }
        public Guid BankAccountId { get; set; }
        //Relations
        public IPGTerminal Terminal { get; set; }
        public BankAccount BankAccount { get; set; }
    }
    public class OnlinePayment : BusinessEntity
    {
        public int RequestNo { get; set; }
        public string? BankReferenceNo { get; set; }   //Token
        public Guid BankId { get; set; }
        public Guid CustomerId { get; set; }
        public int StatusId { get; set; }
        public Guid IPGTerminalId { get; set; }
        public string? TraceNo { get; set; }   //Bank Confirmation No

        //Relations
        public Bank Bank { get; set; }
        public Person Customer { get; set; }
        public OnlinePaymentStatus Status { get; set; }
        public IPGTerminal Terminal { get; set; }

        public ICollection<OnlinePaymentItem> Items { get; set; }
        public ICollection<OnlinePaymentBankAccount> Accounts { get; set; }

    }
    public class OnlinePaymentItem : BusinessEntity
    {
        public Guid OnlinePaymentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? ChargeId { get; set; }
        public long RemainAmount { get; set; }
        public long PaymentAmount { get; set; }

        //Relations
        public OnlinePayment OnlinePayment { get; set; }
        public Invoice? Invoice { get; set; }
        public Charge? Charge { get; set; }
    }
    public class OnlinePaymentBankAccount : BusinessEntity
    {
        public Guid OnlinePaymentId { get; set; }
        public Guid BankAccountId { get; set; }
        public string? BankReferenceNo { get; set; }
        public long Amount { get; set; }
        //Relations
        public BankAccount BankAccount { get; set; }
        public OnlinePayment OnlinePayment { get; set; }
    }
    public class OnlinePaymentStatus : SystemEntity
    {
        public string OnlinePaymentStatusName { get; set; }
        public string? OnlinePaymentStatusLocalName { get; set; }
    }
}
