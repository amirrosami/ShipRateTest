using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class RateRequestValidity : BusinessEntity
    {
        public Guid RateRequestId { get; set; }
        public DateTime ValidDate { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        //Relations
        public RateRequest RateRequest { get; set; }
    }
}