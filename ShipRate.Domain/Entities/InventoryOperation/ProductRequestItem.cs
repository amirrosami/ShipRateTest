using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class ProductRequestItem : BusinessEntity
    {
        public Guid ProductRequestId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        //Relations
        public ProductRequest ProductRequest { get; set; }
        public Product Product { get; set; }
    }
}
