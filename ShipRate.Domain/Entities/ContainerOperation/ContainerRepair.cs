using NetTopologySuite.Geometries;
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerRepair : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ContainerId { get; set; }
        public DateTime RepairDate { get; set; }
        public Guid? RepairById { get; set; }
        public Guid? LocationId { get; set; }
        public string Remarks { get; set; } = string.Empty;

        //Relations
        public Company Company { get; set; }
        public Container Container { get; set; }
        public Person RepairBy { get; set; }
        public Location Location { get; set; }
    }
}