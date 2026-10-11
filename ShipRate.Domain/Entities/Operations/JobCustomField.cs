using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobCustomField : BusinessEntity
    {
        public Guid? JobId { get; set; }
        public int CustomFieldId { get; set; }
        public string FieldValue { get; set; }
        //Relations
        public Job Job { get; set; }
        public CustomField CustomField { get; set; }
    }
}
