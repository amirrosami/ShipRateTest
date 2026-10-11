using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class InventoryProduct : BusinessEntity
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public int Stock { get; set; }
        public int ReOrderLevel { get; set; }

        //Relations
        public Inventory Inventory { get; set; }
        public Product Product { get; set; }

    }
}
