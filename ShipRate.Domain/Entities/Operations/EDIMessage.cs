using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Operations
{
    public class EDIMessage : BusinessEntity
    {
        public int MessageTypeId { get; set; }
        public int InOutId { get; set; }
        public string ReferenceNumber { get; set; }
        public string Payload { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ErrorMessage { get; set; }
        public string Remarks { get; set; }
        //Relations
        public EDIMessageType MessageType { get; set; }
        public InOut InOut { get; set; }
    }
}
