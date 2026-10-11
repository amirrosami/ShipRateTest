using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.Operations
{
    public class TIP : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ShippingLineId { get; set; }
        public Guid VoyagePortId { get; set; }
        public string TIPNo { get; set; } = string.Empty;
        public DateTime TIPDate { get; set; }
        public DateTime TIPExpiryDate { get; set; }
        public Guid CustomsOfficeId { get; set; }
        public int TIPStatusId { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Location? CustomsOffice { get; set; }
        public Carrier ShippingLine { get; set; }
        public VoyagePort VoyagePort { get; set; }
        public TIPStatus TIPStatus { get; set; }
    }
}
