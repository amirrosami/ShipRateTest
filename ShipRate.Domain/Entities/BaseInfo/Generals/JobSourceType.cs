using ShipRate.Domain.Entities.Base;
namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class JobSourceType : SystemEntity
    {
        public string JobSourceTypeName { get; set; } = string.Empty;
        public string JobSourceTypeLocalName { get; set; } = string.Empty;
    }
}