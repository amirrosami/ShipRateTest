using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class HologramSeries : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ShippingLineId { get; set; }
        public Guid BLSerialSeriesId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Remarks { get; set; }


        //Relations
        public Company Company { get; set; }
        public Carrier ShippingLine { get; set; }
        public BLSerialSeries BLSerialSeries { get; set; }
    }
}
