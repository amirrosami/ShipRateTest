using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.Operations
{
    public class Loop : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ShippingLineId { get; set; }
        public Guid? ShippingLineServiceId { get; set; }
        public string LoopName { get; set; } = string.Empty;
        public string LoopCode { get; set; } = string.Empty;
        public string LoopRemarks { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        //Relations 
        public Company Company { get; set; }
        public Carrier ShippingLine { get; set; }
        public ShippingLineService ShippingLineService { get; set; }
        public List<LoopPort> LoopPorts { get; set; }
    }
}