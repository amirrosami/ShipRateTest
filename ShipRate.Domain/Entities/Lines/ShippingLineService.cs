using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.Lines
{
    public class ShippingLineService : BusinessEntity
    {
        public Guid ShippingLineId { get; set; }
        public string ShippingLineServiceName { get; set; } = string.Empty;
        public string ShippingLineServiceCode { get; set; } = string.Empty;
        public string ShippingLineServiceRemarks { get; set; } = string.Empty;

        //Relations
        public Carrier? ShippingLine { get; set; }
        public List<Loop> Loops { get; set; }
    }
}
