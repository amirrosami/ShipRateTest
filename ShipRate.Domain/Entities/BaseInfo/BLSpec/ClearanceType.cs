using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BLSpec
{
    public class ClearanceType : SystemEntity
    {
        public string ClearanceTypeName { get; set; } = string.Empty;
    }
}
