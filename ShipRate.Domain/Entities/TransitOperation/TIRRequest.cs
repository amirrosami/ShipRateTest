using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TIRRequest : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string TIRRequestNumber { get; set; } = string.Empty;
        public DateTime TIRRequestDate { get; set; }
        public Guid CarrierId { get; set; }
        public int TIRRequestTypeId { get; set; }
        public int TIRRequestStatusId { get; set; }
        public string Remarks { get; set; }
        //Relations
        public Carrier Carrier { get; set; }
        public Company Company { get; set; }
    }
}
