using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestInquiry : BusinessEntity
    {
        public Guid RateRequestId { get; set; }
        public string InquiryNo { get; set; }
        public DateTime InquiryDate { get; set; }
        public Guid VendorId { get; set; }
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }
        public int TransportTypeId { get; set; }
        public string? RemarksInInquiry { get; set; }

        public Guid? SentByUserId { get; set; }
        public DateTime? SentDateTime { get; set; }
        public DateTime? ResponseDueDate { get; set; }
        public DateTime? RateRequestAgentSeenDate { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectReason { get; set; } = string.Empty;
        public Guid? RejectedByUserId { get; set; }
        public DateTime? RejectDateTime { get; set; }
        public int StatusId { get; set; }

        //Relations
        public RateRequest RateRequest { get; set; }
        public Person? Vendor { get; set; }
        public ApplicationUser RejectedByUser { get; set; }
        public Location Origin { get; set; }
        public Location Destination { get; set; }
        public InquiryStatus Status { get; set; }
        public TransportType TransportType { get; set; }
        public ApplicationUser SentByUser { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
    }
}