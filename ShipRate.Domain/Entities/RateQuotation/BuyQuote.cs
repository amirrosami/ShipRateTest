using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class BuyQuote : BusinessEntity
    {
        public Guid? RateRequestInquiryId { get; set; }
        public Guid? CarrierId { get; set; }
        public string? QuotationNo { get; set; }

        public Guid? VendorId { get; set; }

        public string? CarrierName { get; set; }
        public string? VendorName { get; set; }

        public DateTime QuotationDate { get; set; }


        public string? OptionsServiceName { get; set; } = string.Empty;

        public string? VehicleName { get; set; }
        public string? TransportMovementNo { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }

        public int? TransitDays { get; set; }
        public bool IsDirect { get; set; } = false;
        public DateTime? ValidUntil { get; set; }
        public string? Remarks { get; set; }

        public int StatusId { get; set; }
        //Relations
        public List<BuyQuoteItem>? BuyQuoteItems { get; set; } = new List<BuyQuoteItem>();
        public RateRequestInquiry? RateRequestInquiry { get; set; }
        public Person? Carrier { get; set; }
        public Person? Vendor { get; set; }
        public QuotationStatus Status { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser EditedByUser { get; set; }


    }
}
