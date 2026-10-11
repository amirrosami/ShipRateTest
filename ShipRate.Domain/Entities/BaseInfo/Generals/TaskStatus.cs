using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TaskStatus : SystemEntity
    {
        public string TaskStatusName { get; set; }
        public string TaskStatusLocalName { get; set; } = string.Empty;
    }
}
