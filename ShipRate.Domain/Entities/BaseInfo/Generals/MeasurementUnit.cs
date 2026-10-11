using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class MeasurementUnit : SystemEntity
    {
        public int MeasurementUnitCategoryId { get; set; }
        public string MeasurementUnitName { get; set; }
        public string MeasurementUnitLocalName { get; set; }
        public bool SystemDefined { get; set; }
        //Relation
        public MeasurementUnitCategory? MeasurementUnitCategory { get; set; }
    }
}
