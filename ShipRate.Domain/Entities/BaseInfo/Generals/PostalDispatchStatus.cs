using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class PostalDispatchStatus : SystemEntity
    {
        public string StatusName { get; set; } = string.Empty;
        public string StatusLocalName { get; set; } = string.Empty;
    }
}
