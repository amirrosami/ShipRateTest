using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class CommentType : SystemEntity
    {
        public string CommentTypeName { get; set; }
        public string CommentTypeLocalName { get; set; } = string.Empty;
    }
}
