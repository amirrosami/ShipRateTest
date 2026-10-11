using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Contract
{
    public class Contract : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ShippingLineId { get; set; }
        public Guid? ShippingLineServiceId { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public string FPNo { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public string ContractTitle { get; set; } = string.Empty;
        public string ContractRemarks { get; set; } = string.Empty;
        public bool IsDirectFP { get; set; }
        public int ContractTypeId { get; set; }

        //Relations
        public Company Company { get; set; }
        public Customer? Customer { get; set; }
        public ShippingLineService? ShippingLineService { get; set; }
        public Carrier? ShippingLine { get; set; }
        public ContractType ContractType { get; set; }
    }
}