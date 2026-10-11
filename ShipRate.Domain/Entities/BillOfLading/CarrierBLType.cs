using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class CarrierBLType : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid CarrierId { get; set; }
        public string CarrierBLTypeName { get; set; } = string.Empty;
        public bool HasHologram { get; set; }
        public bool HasQRCode { get; set; }


        //Relations
        public Company Company { get; set; }
        public Carrier Carrier { get; set; }
    }
}