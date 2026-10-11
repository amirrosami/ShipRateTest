using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class RiskLevel : SystemEntity
    {
        public string RiskLevelName { get; set; } = string.Empty;
    }
}
