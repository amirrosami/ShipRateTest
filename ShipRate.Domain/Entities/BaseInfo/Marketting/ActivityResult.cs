using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class ActivityResult : SystemEntity
    {
        public string ActivityResultName { get; set; } = string.Empty;
        public string ActivityResultLocalName { get; set; } = string.Empty;
    }
}
