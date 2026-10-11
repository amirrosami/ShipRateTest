using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class ProductCategory : BusinessEntity
    {
        public string ProductCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
