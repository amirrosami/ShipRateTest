using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class TreasuryDocument : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string? DocumentNo { get; set; } = default!;
        public int TreasuryDocumentTypeId { get; set; }
        public DateTime DocumentDate { get; set; }
        public Guid? PartyId { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; } = 1;
        public string? Description { get; set; }
        public int TreasuryDocumentStatusId { get; set; }

        //relations
        public TreasuryDocumentType TreasuryDocumentType { get; set; }
        public TreasuryDocumentStatus Status { get; set; }
        public Person? Party { get; set; }
        public Currency? Currency { get; set; }
        public Company? Company { get; set; }

        public virtual ICollection<TreasuryDocumentItem> TreasuryDocumentItems { get; set; }
            = new List<TreasuryDocumentItem>();
    }


    public class TreasuryDocumentItem : BusinessEntity
    {
        public Guid TreasuryDocumentId { get; set; }
        public Guid PaymentDocumentId { get; set; }

        public decimal Amount { get; set; }
        public int CurrencyId { get; set; } = 1;

        public string? Description { get; set; }

        //Relations
        public virtual TreasuryDocument TreasuryDocument { get; set; } = default!;
        public virtual PaymentDocument PaymentDocument { get; set; } = default!;
        public Currency? Currency { get; set; }
    }



}
