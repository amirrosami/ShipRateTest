using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class SellQuote : BusinessEntity
    {
        public int CompanyId { get; set; }

        public string QuotationNo { get; set; }
        public DateTime QuotationDate { get; set; }

        public Guid? SelectedBuyQuoteId { get; set; }
        public Guid RateRequestId { get; set; }

        public string? ServiceName { get; set; } = string.Empty;
        public string? VehicleNames { get; set; }
        public string? MovementNumbers { get; set; }

        public DateTime ETD { get; set; }
        public DateTime ETA { get; set; }

        public DateTime ValidUntil { get; set; }
        public string? Remarks { get; set; }
        public int StatusId { get; set; }

        public Guid? SentByUserId { get; set; }
        public DateTime? SentByUserDate { get; set; }

        public Guid? CustomerRespondUserId { get; set; }
        public DateTime? CustomerRespondDate { get; set; }
        public string? CustomerRespondComments { get; set; }

        public Guid? CarrierId { get; set; }

        //Relations
        public Company Company { get; set; }
        public RateRequest RateRequest { get; set; }
        public BuyQuote? SelectedBuyQuote { get; set; }
        public QuotationStatus Status { get; set; }
        public Carrier? Carrier { get; set; }

        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser LastEditUser { get; set; }
        public ApplicationUser SentByUser { get; set; }

        public List<SellQuoteItem> Items { get; set; } = new List<SellQuoteItem>();
    }
}
