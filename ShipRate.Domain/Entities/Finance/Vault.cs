using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public enum EnumCashPaymentRequestStatus
    {
        Draft = -1,
        Pending = 1,       // در انتظار پرداخت توسط مسئول صندوق
        Paid = 2,          // پرداخت شده و سند مالی صادر شده
        Rejected = 3,      // رد شده توسط صندوق‌دار یا مدیر
        Cancelled = 4,      // لغو شده توسط متقاضی
        InitialConfirmed = 5,      // تایید اولیه
        FinalConfirmed = 6      // تایید نهایی شده
    }
    public enum EnumVaultItemCategory
    {
        TitleDeed = 1,          // سند مالکیت ملکی (زمین، دفتر، انبار)
        VesselDocument = 2,     // اسناد مالکیتی و فنی شناور/کشتی
        OriginalBL = 3,         // بارنامه‌های کاغذی اصل (OBL)
        BankGuarantee = 4,      // ضمانت‌نامه‌های بانکی حسن انجام کار/تعهدات
        PromissoryNote = 5,     // سفته‌های ضمانت (پرسنل/نمایندگان)
        GuaranteeCheque = 6,    // چک‌های امانی/ضمانتی
        CorporateCertificate = 7,// روزنامه رسمی، آگهی تأسیس، پروانه فعالیت
        HardwareDevice = 8,     // توکن‌های بانکی، مهر شرکت، دانگل‌ها
        Other = 99              // سایر اوراق مهم
    }
    public enum EnumVaultItemStatus
    {
        InVault = 1,       // موجود در گاوصندوق
        CheckedOut = 2,    // به امانت رفته (خروج موقت)
        PermanentOut = 3,  // خروج قطعی (ابطال/عودت همیشگی به طرف‌حساب)
        Archived = 4       // بایگانی راکد
    }
    public enum EnumVaultCustodyActionType
    {
        InitialDeposit = 1, // ورود و پذیرش اولیه به صندوق
        BorrowCheckOut = 2, // خروج امانی (تحویل به شخص)
        ReturnCheckIn = 3,  // عودت به صندوق
        PermanentOut = 4    // خروج دائمی و خاتمه
    }
    public enum EnumVaultMovementSourceType
    {
        CustomerReceipt = 1,        // سند دریافت نقدی از مشتری (PaymentDocument)
        BankToVault = 2,            // شارژ گاوصندوق از بانک (برداشت از حساب بانکی)
        PettyCashSettlement = 3,    // عودت مانده تنخواه پرسنل/پیک
        InterVaultTransferIn = 4,   // ورود ناشی از انتقال از گاوصندوق دیگر
        DepositRefundIn = 5,        // استرداد سپرده/دپازیت نقدی دریافتی
        PhysicalSurplus = 6,        // مازاد ناشی از شمارش فیزیکی (تعدیل مثبت)
        CapitalInjection = 7,       // تزریق سرمایه نقدی توسط شرکا/مدیریت

        CashPaymentRequest = 10,    // پرداخت نقدی بر اساس درخواست پرداخت
        VaultToBank = 11,           // واریز وجه نقد به حساب بانکی
        PettyCashAdvance = 12,      // پرداخت علی‌الحساب تنخواه
        InterVaultTransferOut = 13, // خروج ناشی از انتقال به گاوصندوق دیگر
        PhysicalShortage = 14       // کسری ناشی از شمارش فیزیکی (تعدیل منفی)
    }


    public class VaultMovementSourceType : SystemEntity
    {
        public string VaultMovementSourceTypeName { get; set; } = string.Empty;
        public string VaultMovementSourceTypeLocalName { get; set; } = string.Empty;
    }
    public class CashPaymentRequestStatus : SystemEntity
    {
        public string CashPaymentRequestStatusName { get; set; } = string.Empty;
        public string CashPaymentRequestStatusLocalName { get; set; } = string.Empty;
    }
    public class VaultItemCategory : SystemEntity
    {
        public string VaultItemCategoryName { get; set; } = string.Empty;
        public string VaultItemCategoryLocalName { get; set; } = string.Empty;
    }
    public class VaultItemStatus : SystemEntity
    {
        public string VaultItemStatusName { get; set; } = string.Empty;
        public string VaultItemStatusLocalName { get; set; } = string.Empty;
    }
    public class VaultCustodyActionType : SystemEntity
    {
        public string VaultCustodyActionTypeName { get; set; } = string.Empty;
        public string VaultCustodyActionTypeLocalName { get; set; } = string.Empty;
    }


    public class Vault : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public string VaultName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Guid? KeeperUserId { get; set; }

        //Relartions
        public Company Company { get; set; }
        public Branch Branch { get; set; }

        public ApplicationUser? KeeperUser { get; set; }
        public ICollection<VaultAsset> Assets { get; set; } = new List<VaultAsset>();
        public ICollection<VaultMovement> Movements { get; set; } = new List<VaultMovement>();
        public ICollection<VaultItem> Items { get; set; } = new List<VaultItem>();

    }
    public class VaultAsset : BusinessEntity
    {
        public Guid VaultId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        //Relation
        public Vault Vault { get; set; }
        public Currency Currency { get; set; }
    }
    public class VaultMovement : BusinessEntity
    {
        public Guid VaultId { get; set; }
        public int CurrencyId { get; set; }
        public int InOutId { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public int VaultMovementSourceTypeId { get; set; }

        public Guid? SourceEntityId { get; set; }           // ارجاع به سند دریافتی، درخواست پرداخت یا انتقال
        public string? ReferenceNo { get; set; }            // شماره پیگیری/فیش/رسید فیزیکی
        public string? DepositorOrReceiver { get; set; }    // نام شخص واریزکننده یا دریافت‌کننده
        public string? Remarks { get; set; }


        public Guid? ExecutedByUserId { get; set; }
        public DateTime? MovementDate { get; set; }

        //Relations
        public Vault Vault { get; set; }
        public Currency Currency { get; set; }
        public ApplicationUser? ExecutedByUser { get; set; }
        public VaultMovementSourceType SourceType { get; set; }
        public InOut InOut { get; set; }
    }
    public class CashPaymentRequest : BusinessEntity
    {
        public string RequestNo { get; set; } = null!;       // مثلا: CPR-1403-0001
        public Guid VaultId { get; set; }
        public Guid? PartyId { get; set; }                   // طرف‌حساب رسمی (اختیاری)
        public string? PayeeName { get; set; }               // نام شخص دریافت‌کننده وجه

        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string Reason { get; set; } = null!;          // بابت چیست؟

        public int StatusId { get; set; }

        public Guid? InitialConfirmedByUserId { get; set; }
        public DateTime? InitialConfirmDate { get; set; }

        public Guid? FinalConfirmedByUserId { get; set; }
        public DateTime? FinalConfirmDate { get; set; }

        public Guid? PaidByUserId { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? RejectReason { get; set; }

        // پیوند به سند مالی تولید شده و لاگ گردش
        public Guid? PaymentDocumentId { get; set; }
        public Guid? VaultMovementId { get; set; }

        // Navigation Properties
        public Vault Vault { get; set; } = null!;
        public Person? Party { get; set; }
        public Currency Currency { get; set; } = null!;
        public ApplicationUser? CreateUser { get; set; }
        public ApplicationUser? InitialConfirmUser { get; set; }
        public ApplicationUser? FinalConfirmUser { get; set; }
        public ApplicationUser? PaidByUser { get; set; }
        public PaymentDocument? PaymentDocument { get; set; }
        public VaultMovement? VaultMovement { get; set; }
        public CashPaymentRequestStatus CashPaymentRequestStatus { get; set; }
    }
    public class VaultItem : BusinessEntity
    {
        public Guid VaultId { get; set; }
        public int VaultItemCategoryId { get; set; }
        public string Title { get; set; } = null!;           // نام یا عنوان سند (مثلاً: سند تک‌برگ ملک دفتر مرکزی)
        public string? IdentifierNo { get; set; }           // شماره پلاک ثبتی، شماره ضمانت‌نامه، شماره چک
        public string? OwnerOrBeneficiary { get; set; }     // مالک یا ذی‌نفع
        public decimal? NominalValue { get; set; }          // مبلغ اسمی / ارزش ضمانت
        public int? CurrencyId { get; set; }

        public DateTime? IssueDate { get; set; }            // تاریخ صدور
        public DateTime? ExpiryDate { get; set; }           // تاریخ سررسید / انقضا (برای چک، ضمانت‌نامه و مجوزها)

        public string? ShelfLocation { get; set; }          // موقعیت فیزیکی: زونکن A-2، طبقه سوم
        public int VaultItemStatusId { get; set; }

        // اطلاعات وضعیت امانت (در صورت CheckedOut بودن)
        public Guid? CurrentHolderUserId { get; set; }
        public string? CurrentHolderExternalName { get; set; }
        public DateTime? CheckedOutDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }   // موعد مقرر برای بازگرداندن

        public string? AttachmentPath { get; set; }         // مسیر فایل اسکن سند (PDF/Image)
        public string? Description { get; set; }

        // Navigation Properties
        public Vault Vault { get; set; } = null!;
        public VaultItemCategory VaultItemCategory { get; set; }
        public VaultItemStatus VaultItemStatus { get; set; }
        public Currency? Currency { get; set; }
        public ApplicationUser? CurrentHolderUser { get; set; }
        public ICollection<VaultItemCustodyLog> CustodyLogs { get; set; } = new List<VaultItemCustodyLog>();
    }
    public class VaultItemCustodyLog : BusinessEntity
    {
        public Guid VaultItemId { get; set; }
        public int VaultCustodyActionTypeId { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;

        public Guid ExecutedByUserId { get; set; }          // امین صندوقی که تحویل داده یا دریافت کرده
        public Guid? HandedToUserId { get; set; }           // پرسنل تحویل‌گیرنده (در امانت)
        public string? HandedToExternalName { get; set; }   // شخص خارجی تحویل‌گیرنده (وکیل، نماینده دفترخانه و...)

        public string Reason { get; set; } = null!;         // دلیل تحویل یا عودت
        public DateTime? ExpectedReturnDate { get; set; }   // موعد بازگشت تعهد شده
        public string? HandoverReceiptScanPath { get; set; } // تصویر فرم/رسید امضاشده تحویل فیزیکی

        // Navigation Properties
        public VaultItem VaultItem { get; set; } = null!;
        public ApplicationUser ExecutedByUser { get; set; } = null!;
        public ApplicationUser? HandedToUser { get; set; }
        public VaultCustodyActionType VaultCustodyActionType { get; set; }
    }

}