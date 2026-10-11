using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.DetentionCalculation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
using System.ComponentModel.DataAnnotations.Schema;
namespace ShipRate.Domain.Entities.Finance
{

    public class InvoiceModel : SystemEntity
    {
        public string InvoiceModelName { get; set; } = string.Empty;
        public string? InvoiceModelLocalName { get; set; }
    }


    public class InvoiceSubject : SystemEntity
    {
        public string InvoiceSubjectName { get; set; } = string.Empty;
        public string? InvoiceSubjectLocalName { get; set; }
    }
    public class Invoice : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int FinanceDirectionId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int InvoiceFNFId { get; set; }
        public int InvoiceModelId { get; set; }

        public Guid? BranchId { get; set; }

        public int InvoiceSubjectId { get; set; }
        public string? InvoiceSubjectTitle { get; set; }

        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }

        public Guid? JobId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }

        public Guid? ParentInvoiceId { get; set; }
        public Guid? DetentionId { get; set; }

        public Guid? PartyId { get; set; }
        public int CurrencyId { get; set; }

        public bool? IsReturn { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal FirstTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }

        [NotMapped]
        public decimal RemainingAmount => TotalAmount - SettledAmount;

        [NotMapped]
        public bool IsFullySettled => RemainingAmount <= 0;



        public string Remarks { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public decimal? DefaultTaxPercent { get; set; }

        public int InvoiceStatusId { get; set; }

        public Guid? ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public bool IsPostedToAcc { get; set; }
        public DateTime PostedToAcc { get; set; }

        public int Serial { get; set; }
        public string SerialPrefix { get; set; } = string.Empty;

        [NotMapped]
        public string SerialDisplay => Serial <= 0 ? "-" : (string.IsNullOrWhiteSpace(SerialPrefix) ? Serial.ToString() : $"{SerialPrefix} {Serial}");
        //Relations
        public Company Company { get; set; }

        public Job Job { get; set; }
        public Booking Booking { get; set; }
        public BL BL { get; set; }
        public Branch Branch { get; set; }

        public Detention? Detention { get; set; }
        public Person? Party { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }

        public InvoiceType? InvoiceType { get; set; }
        public InvoiceModel? InvoiceModel { get; set; }

        public Currency? Currency { get; set; }

        public ApplicationUser ApprovedBy { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser UpdateUser { get; set; }

        public FinanceDirection FinanceDirection { get; set; }
        public InvoiceFNF? InvoiceFNF { get; set; }
        public InvoiceSubject InvoiceSubject { get; set; }




        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();
    }
    public class InvoiceItem : BusinessEntity
    {
        public Guid InvoiceId { get; set; }
        public int SequenceNumber { get; set; }
        public int ChargeTypeId { get; set; }
        public Guid? ChargeId { get; set; }
        public string Description { get; set; }
        public decimal QTY { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AmountBeforeDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public string ChargeTypeSupplementRemarks { get; set; }   //Takmil Konande ye noe hazineh dar print va froms 

        //Relations
        public Invoice? Invoice { get; set; }
        public ChargeType? ChargeType { get; set; }
        public Charge? Charge { get; set; }
        public Currency Currency { get; set; }
        public ICollection<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();
    }


    public class InvoiceFNF : SystemEntity
    {
        public string FNFName { get; set; } = string.Empty;
    }















}