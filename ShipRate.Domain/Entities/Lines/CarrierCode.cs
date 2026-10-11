using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Lines
{
    public class CarrierCode : BusinessEntity
    {
        public Guid CarrierId { get; set; }
        public string CodeType { get; set; } // SCAC / IATA / Customs /ICAO
        public string CodeValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }


        //Relations
        public Carrier Carrier { get; set; }
    }
}