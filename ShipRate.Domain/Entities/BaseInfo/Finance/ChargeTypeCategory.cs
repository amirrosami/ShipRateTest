using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class ChargeTypeCategory : SystemEntity
    {
        public string ChargeTypeCategoryName { get; set; } = string.Empty;
        public string ChargeTypeCategoryLocalName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        //Relations
        public List<ChargeType>? ChargeTypes { get; set; }
    }
}
