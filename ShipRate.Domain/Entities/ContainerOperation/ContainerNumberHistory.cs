using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerNumberHistory : BusinessEntity
    {
        public Guid ContainerId { get; set; }
        public string OldContainerNumber { get; set; } = string.Empty;
        public string NewContainerNumber { get; set; } = string.Empty;
        public DateTime ChangeDateTime { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        //Relations

        public Container Container { get; set; }
    }
}
