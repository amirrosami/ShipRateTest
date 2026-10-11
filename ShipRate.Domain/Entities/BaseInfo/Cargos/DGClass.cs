using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class DGClass : SystemEntity
    {
        public string DGClassNo { get; set; } = string.Empty;
        public string DGClassName { get; set; } = string.Empty;
        public string DGClassDescription { get; set; } = string.Empty;
    }
}
