using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class Hologram : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid HologramSeriesId { get; set; }
        public Guid? BLSerialId { get; set; }
        public string HologramNumber { get; set; } = string.Empty;   //LBA-000123
        public string EncryptedSerial { get; set; }
        public string SerialHash { get; set; }
        public int HologramStatusId { get; set; }

        //Relation
        public Company Company { get; set; }
        public HologramSeries HologramSeries { get; set; }
        public HologramStatus HologramStatus { get; set; }
        public BLSerial? BLSerial { get; set; }
    }
}
