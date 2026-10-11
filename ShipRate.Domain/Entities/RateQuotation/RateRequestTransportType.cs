using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestTransportType : BusinessEntity
    {
        public Guid RateRequestId { get; set; }
        public int Sequence { get; set; }
        public int TransportTypeId { get; set; }
        //Relations
        public RateRequest RateRequest { get; set; }
        public TransportType TransportType { get; set; }
    }
}
