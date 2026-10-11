using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class InfoType : SystemEntity
    {
        public string InfoTypeName { get; set; } = string.Empty;
        public int InfoTypeCategoryId { get; set; }

        //Relation 
        public InfoTypeCategory? InfoTypeCategory { get; set; }
    }
}
