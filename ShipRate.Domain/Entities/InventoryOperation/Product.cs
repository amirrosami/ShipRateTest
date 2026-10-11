using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class Product : BusinessEntity
    {
        public Guid ProductCategoryId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal ProductMinStock { get; set; }
        public bool IsActive { get; set; } = true;


        //Relations
        public ProductCategory ProductCategory { get; set; }
    }
}
