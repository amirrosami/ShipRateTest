using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class CustomsStatus : SystemEntity
    {
        public string CustomsStatusName { get; set; }
        public string CustomsStatusLocalName { get; set; }
    }
}
