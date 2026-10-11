using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class CalculationType : SystemEntity
    {
        public string CalculationTypeName { get; set; } = string.Empty;
    }
}
