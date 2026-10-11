using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class TelexMessage : BusinessEntity
    {
        public Guid BLId { get; set; }
        public Guid TelexReleaseRequestId { get; set; }
        public int? SendToolTypeId { get; set; }

        public string? Recipient { get; set; }
        public string? Subject { get; set; }
        public string? MessageBody { get; set; }

        public DateTime? SendDate { get; set; }
        public Guid? SentByUserId { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        //Relations
        public BL BL { get; set; }
        public TelexReleaseRequest TelexReleaseRequest { get; set; }
        public SendToolType SendToolType { get; set; }
    }
}
