using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonalityType : SystemEntity
    {
        public string PersonalityTypeName { get; set; } = string.Empty;
    }
}
