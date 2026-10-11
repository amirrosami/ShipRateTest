using ShipRate.Domain.Entities.Finance;

namespace ShipRate.Application.ViewModels
{

    public class OnlinePaymentFilterVm
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string RequestNo { get; set; }
        public string InvoiceNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class CreateOnlinePaymentVm
    {
        public Guid? Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid IPGTerminalId { get; set; }
        public List<CreateOnlinePaymentItemVm> Items { get; set; }
        public CreateOnlinePaymentVm()
        {
            
        }
        public CreateOnlinePaymentVm(Guid? id, Guid customerId,List<CreateOnlinePaymentItemVm> items)
        {
            Id = id;
            CustomerId = customerId;
            Items = items;
        }
    }
    public class OnlinePaymentListVM
    {
        public Guid Id { get; set; }
        public int RequestNo { get; set; }
        public string? BankReferenceNo { get; set; }   //Token
        public string? TraceNo { get; set; }
        //Relations
        public string Status { get; set; }
        public string TerminalName { get; set; }
        public long TotalPayment {  get; set; }
    }

    public class OnlinePaymentItemsVm
    {
        public Guid Id { get; set; }
        public long PaymentAmount { get; set; }
        //Relations
        public string InvoiceNo { get; set; }
    }

    public class CreateOnlinePaymentItemVm
    {
        public Guid? OnlinePaymentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? ChargeId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public long RemainAmount { get; set; }
        public long PaymentAmount { get; set; }
        public string InvoiceNo { get; set; }
        public long Total { get; set; }
        public CreateOnlinePaymentItemVm()
        {
            
        }

        public CreateOnlinePaymentItemVm(Guid? onlinePaymentId, Guid? invoiceId, Guid? chargeId, DateTime invoiceDate, long remainAmount, long paymentAmount, string invoiceNo, long total)
        {
            OnlinePaymentId = onlinePaymentId;
            InvoiceId = invoiceId;
            ChargeId = chargeId;
            InvoiceDate = invoiceDate;
            RemainAmount = remainAmount;
            PaymentAmount = paymentAmount;
            InvoiceNo = invoiceNo;
            Total = total;
        }
    }

    public class CreateOnlinePaymentBankAccountvm
    {
        public Guid Id { get; set; }
        public Guid OnlinePaymentId { get; set; }
        public Guid BankAccountId { get; set; }
        public string? BankReferenceNo { get; set; }
        public long Amount { get; set; }
    }

}
