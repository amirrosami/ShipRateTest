using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Customers;

namespace ShipRate.Domain.Entities.Operations
{
    public class ProofOfDelivery : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? ShipmentTransitId { get; set; }
        public Guid? BLId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public byte[] Signature { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Shipment? Shipment { get; set; }
        public BL? BL { get; set; }
        public Customer Customer { get; set; }
    }
}
