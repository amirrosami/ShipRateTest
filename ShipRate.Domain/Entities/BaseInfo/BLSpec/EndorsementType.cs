using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BLSpec
{
    public class EndorsementType : SystemEntity
    {
        public string EndorsementTypeName { get; set; }
        public string EndorsementTypeLocalName { get; set; }
    }
}
