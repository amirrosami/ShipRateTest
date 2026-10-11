using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.MyInquiriesIndexVMs.cs
{
    // ViewModels/MyInquiriesIndexVM.cs
    public class MyInquiriesIndexVM
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }
        public string InquiryNo { get; set; }
        public string InquiryDate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CargoType { get; set; }
        public decimal? Weight { get; set; }
        public string Status { get; set; } // Draft, Sent, Responded, Rejected
        public DateTime SentDate { get; set; }
        public bool HasResponse { get; set; }
    }

    // ViewModels/MyInquiryDetailsVM.cs
    public class MyInquiryDetailsVM
    {
        public Guid InquiryId { get; set; }
        public Guid RateRequestId { get; set; }

        // Rate Request Info
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CargoType { get; set; }
        public decimal? Weight { get; set; }
        public DateTime RequestDate { get; set; }

        // Transport Types
        public List<TransportTypeDto> TransportTypes { get; set; }

        // Packages
        public List<PackageDto> Packages { get; set; }

        // Containers
        public List<ContainerDto> Containers { get; set; }

        // Inquiry Info
        public string Status { get; set; }
        public DateTime SentDate { get; set; }

        // Existing Quote (if any)
        public BuyQuoteDto ExistingQuote { get; set; }
    }

    public class TransportTypeDto
    {
        public string Type { get; set; }
    }

    public class PackageDto
    {
        public string Type { get; set; }
        public int Quantity { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }

    public class ContainerDto
    {
        public string Type { get; set; }
        public decimal Quantity { get; set; }
    }

    public class BuyQuoteDto
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public string VesselName { get; set; }
        public string VoyageNumber { get; set; }
        public DateTime ETD { get; set; }
        public DateTime ETA { get; set; }
        public int TransitDays { get; set; }
        public bool IsDirect { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Remarks { get; set; }
        public List<BuyQuoteItemDto> Items { get; set; }
    }

    public class BuyQuoteItemDto
    {
        public string ChargeType { get; set; }
        public string Currency { get; set; }
        public string ContainerType { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
    }

    // ViewModels/SubmitQuoteVM.cs
    public class SubmitQuoteVM
    {
        public Guid InquiryId { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string VesselName { get; set; }

        [Required]
        public string VoyageNumber { get; set; }

        [Required]
        public DateTime ETD { get; set; }

        [Required]
        public DateTime ETA { get; set; }

        [Required]
        public int TransitDays { get; set; }

        public bool IsDirect { get; set; }

        public DateTime? ValidUntil { get; set; }

        public string Remarks { get; set; }

        // Quote Items (dynamic grid)
        public List<QuoteItemVM> Items { get; set; } = new();
    }

    public class QuoteItemVM
    {
        public Guid? RateRequestContainerId { get; set; }

        [Required]
        public int ChargeTypeId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public decimal UnitRate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public string Remarks { get; set; }
    }

    // ViewModels/RejectInquiryVM.cs
    public class RejectInquiryVM
    {
        public Guid InquiryId { get; set; }

        [Required]
        public string RejectionReason { get; set; }
    }
}
