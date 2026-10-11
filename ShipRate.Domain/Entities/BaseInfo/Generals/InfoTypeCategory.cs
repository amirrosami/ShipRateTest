using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class InfoTypeCategory : SystemEntity
    {
        public string InfoTypeCategoryName { get; set; } = string.Empty;

        //Relation 

        public List<InfoType>? InfoTypes { get; set; }

    }
}
