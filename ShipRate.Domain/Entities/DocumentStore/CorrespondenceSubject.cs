using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.DocumentStore
{
    public class CorrespondenceSubject : SystemEntity
    {
        public string SubjectTitle { get; set; } = string.Empty;
        public int? ObjectTypeId { get; set; }
        public bool SystemDefined { get; set; }

        //Relations
        public SystemObjectType ObjectType { get; set; }
    }
}
