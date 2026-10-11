using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TransitPermit : BusinessEntity
    {
        public Guid TransitOrderId { get; set; }
        public string PermitNumber { get; set; }
        public DateTime PermitDate { get; set; }
        public DateTime ValidUntil { get; set; }
        public string PermitType { get; set; }   //????
        public string Remarks { get; set; }
        //Relations
    }
}

