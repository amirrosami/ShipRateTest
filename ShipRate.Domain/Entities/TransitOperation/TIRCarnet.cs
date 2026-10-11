using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TIRCarnet : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string TIRCarnetNumber { get; set; } = string.Empty;
        public int IssuingCountryId { get; set; }
        public int IssuingAssociationId { get; set; }
        public DateTime? BuyDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int VoletsCount { get; set; }

        public string? GuaranteeChainName { get; set; } //اتحادیه ضمانت کننده
        public decimal GuaranteeAmount { get; set; }
        public int? CurrencyId { get; set; }

        public DateTime? ReceivedFromDriverAt { get; set; }
        public DateTime? SubmittedToChamberAt { get; set; }
        public DateTime? SettledAt { get; set; }
        public int StatusId { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Association IssuingAssociation { get; set; } //انجمن صادر کننده
        public Country Country { get; set; }
        public Currency Currency { get; set; }
        public TIRCarnetStatus TIRCarnetStatus { get; set; }
        public Company Company { get; set; }
    }
}
