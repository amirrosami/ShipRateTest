using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class TIPUsage : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid TIPId { get; set; }

        public Guid BLId { get; set; }
        public string? BLNo { get; set; }

        public Guid? CustomerId { get; set; }

        public string LetterNo { get; set; }
        public DateTime LetterDate { get; set; }

        public int F20 { get; set; }
        public int F40 { get; set; }
        public string? Remarks { get; set; }
        public int Serial { get; set; }

        //Relations
        public Company Company { get; set; }
        public TIP TIP { get; set; }
        public Person Customer { get; set; }
        public BL BL { get; set; }
    }
    public class TIPUsageContainer : BusinessEntity
    {
        public Guid TIPUsageId { get; set; }
        public Guid? ContainerId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int SizeTypeId { get; set; }

        //Relations
        public TIPUsage TIPUsage { get; set; }
        public Container Container { get; set; }
        public ContainerSizeType SizeType { get; set; }
    }
}