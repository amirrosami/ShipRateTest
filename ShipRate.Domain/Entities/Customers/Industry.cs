using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Customers
{
    public class Industry : BusinessEntity
    {
        public string IndustryName { get; set; } = string.Empty;
    }
}
