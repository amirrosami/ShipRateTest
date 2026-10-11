using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.BillOfLading
{
    public class TelexReleaseRequest : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public int LineForwarderTypeId { get; set; }
        public string? RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public int TelexReleaseStatusId { get; set; }
        public Guid? BLSurrenderId { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public TelexReleaseStatus TelexReleaseStatus { get; set; }
        public BLSurrender? BLSurrender { get; set; }
        public BL? BL { get; set; }
        public Company Company { get; set; }
        public Carrier? ShippingLine { get; set; }
        public Person? Customer { get; set; }
        public Shipment? Shipment { get; set; }
        public LineForwarderType LineForwarderType { get; set; }
    }
}
