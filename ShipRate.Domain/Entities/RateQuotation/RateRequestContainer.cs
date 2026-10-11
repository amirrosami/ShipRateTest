using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestContainer : BusinessEntity
    {
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public int? CntOwnTypeId { get; set; }
        public decimal Counts { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public Job? Job { get; set; }
        public RateRequest? RateRequest { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public CntOwnType CntOwnType { get; set; }
    }
}
