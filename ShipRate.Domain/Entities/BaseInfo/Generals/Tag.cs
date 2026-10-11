using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class Tag : SystemEntity
    {
        public string TagTitle { get; set; } = string.Empty;
    }
}
