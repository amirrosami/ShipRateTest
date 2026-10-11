using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class DGPackingGroup : SystemEntity
    {
        public string DGPackingGroupCode { get; set; } = string.Empty;
        public string DGPackingGroupDescription { get; set; } = string.Empty;
    }
}
