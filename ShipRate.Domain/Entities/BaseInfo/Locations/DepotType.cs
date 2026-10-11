using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Locations
{
    public class DepotType : SystemEntity
    {
        public string DepotTypeName { get; set; } = string.Empty;
    }
}


// CFS, ICD, Empty Depot, Bonded