using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Finance
{

    public class ChequePurpose : SystemEntity
    {
        public string ChequePurposeName { get; set; }
        public string ChequePurposeLocalName { get; set; }
    }


    public class Cheque : BusinessEntity
    {
        public Guid PaymentDocumentId { get; set; }
        public int ChequePurposeId { get; set; }

        public int ChequeTypeId { get; set; }

        public string ChequeNo { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public string BankBranchCode { get; set; } = string.Empty;
        public string BankBranchName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public string? IssuerName { get; set; }
        public string? ReceiverName { get; set; }
        public Guid? BankAccountId { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IBAN { get; set; }
        public string? SayadNo { get; set; }
        public int StatusId { get; set; }

        public string Remarks { get; set; } = string.Empty;

        //Relations
        public PaymentDocument? PaymentDocument { get; set; }
        public Bank? Bank { get; set; }
        public BankAccount? BankAccount { get; set; }
        public Currency? Currency { get; set; }
        public ChequeType? ChequeType { get; set; }
        public ChequeStatus Status { get; set; }
        public ChequePurpose ChequePurpose { get; set; }
    }
}




/*
 * 
 * الف) چرخه چک‌های دریافتی (Received):

    ثبت و دریافت: InVault (موجود در صندوق)
    خواباندن به حساب: InVault →→ InCollection (در جریان وصول)
    وصول شدن: InCollection →→ Cleared
    برگشت خوردن: InCollection →→ Bounced
    خرج کردن/واگذاری به غیر: InVault →→ Endorsed
    استرداد به مشتری: InVault / Bounced →→ ReturnedToDrawer
    اگر امانی باشد: InVault →→ HeldAsGuarantee (نزد شرکت ماندگار تا اتمام قرارداد)

ب) چرخه چک‌های پرداختی (Issued):

    صدور چک: Issued (تحویل به ذینفع)
    پاس شدن از حساب بانک: Issued →→ Cleared
    برگشت خوردن توسط ذینفع: Issued →→ Bounced
    پس گرفتن لاشه چک: Bounced / Issued →→ ReturnedToDrawer →→ Cancelled
    اگر امانی داده شده: Issued →→ HeldAsGuarantee (تحویل طرف مقابل داده شده به‌عنوان ضمانت)

*/