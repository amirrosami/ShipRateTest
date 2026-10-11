using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerUse : BusinessEntity
    {
        public Guid ContainerId { get; set; }
        public int CompanyId { get; set; }

        public Guid? BookingId { get; set; }
        public Guid? RTSInstructionId { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;
        public string? SealNumber { get; set; }
        public string? SecondSealNumber { get; set; }
        public string? EIRNumber { get; set; }

        public long? GrossWeight { get; set; }
        public long? TareWeight { get; set; }
        public int CntOwnTypeId { get; set; }
        public int FullMTId { get; set; }

        public bool IsHazardous { get; set; }
        public Guid? DGId { get; set; }

        public bool IsOverSized { get; set; }
        public int? OverWidth { get; set; }
        public int? OverHeight { get; set; }
        public int? OverLength { get; set; }

        public int ContainerSizeTypeId { get; set; }
        public int? NormalTemp { get; set; }
        public int? MaxTemp { get; set; }
        public int? MinTemp { get; set; }


        public DateTime CycleStartDate { get; set; } = DateTime.Now;
        public bool CycleClosed { get; set; } = false;
        public DateTime? CycleCloseDate { get; set; }
        public string? CycleCloseReason { get; set; }

        //Relations
        public Container? Container { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public CntOwnType? CntOwnType { get; set; }
        public Company Company { get; set; }
        public FullMT FullMT { get; set; }
        public Booking Booking { get; set; }
        public RTSInstruction RTSInstruction { get; set; }

    }
}
