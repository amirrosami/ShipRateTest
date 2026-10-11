using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class Container : BusinessEntity
    {
        public string ContainerNumber { get; set; } = string.Empty;
        public int ContainerSizeTypeId { get; set; }
        public int CntOwnTypeId { get; set; }
        public int OwnLeaseTypeId { get; set; }
        public int MaxPayLoad { get; set; }
        public int TareWeight { get; set; }
        public Guid? OwnerId { get; set; }   //مال اشخاص
        public bool ResponsibilityByUs { get; set; }
        public int? ResponsibleCompanyId { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public CntOwnType? CntOwnType { get; set; }
        public CntOwnLeaseType? CntOwnLeaseType { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public Person? Owner { get; set; }
        public Company? ResponsibleCompany { get; set; }
    }
}
