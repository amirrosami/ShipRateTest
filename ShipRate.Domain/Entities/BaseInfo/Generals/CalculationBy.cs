using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class CalculationBy : SystemEntity
    {
        public string CalculationByName { get; set; } = string.Empty;
    }
}
