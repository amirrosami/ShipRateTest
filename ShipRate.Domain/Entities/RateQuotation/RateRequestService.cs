using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestService : BusinessEntity
    {
        public Guid? RateRequestId { get; set; }
        public Guid? RateRequestInquiryId { get; set; }
        public int ServiceTypeId { get; set; }
        //Relations
        public RateRequest RateRequest { get; set; }
        public RateRequestInquiry RateRequestInquiry { get; set; }
        public ServiceType ServiceType { get; set; }
        public Location Origin { get; set; }
        public Location Destination { get; set; }
    }
}