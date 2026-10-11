using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLSerialSeries : BusinessEntity
    {
        public Guid CarrierBLTypeId { get; set; }
        public string SeriesCode { get; set; }   // مثلاً HLG-2026-A
        public string SeriesName { get; set; }   // مثلاً HLG-2026-A
        public string Prefix { get; set; }       // LBA
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public CarrierBLType CarrierBLType { get; set; }
    }

    public class BLSerialBatchAssign : BusinessEntity
    {
        public Guid BLSerialSeriesId { get; set; }
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime AssignDate { get; set; }
        public Guid AgentId { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public BLSerialSeries BLSerialSeries { get; set; }
        public Person? Agent { get; set; }

    }



    public class BLSerial : BusinessEntity
    {
        public Guid BLSerialSeriesId { get; set; }
        public long SerialNumber { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? BLId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AttachedAt { get; set; }
        //Relations
        public BLSerialSeries BLSerialSeries { get; set; }
        public BL BL { get; set; }
        public Person? Agent { get; set; }
    }

}
