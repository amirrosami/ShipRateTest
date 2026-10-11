using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class StuffingType : SystemEntity
    {
        public string StuffingTypeName { get; set; }
        public string StuffingTypeLocalName { get; set; }
    }
}
