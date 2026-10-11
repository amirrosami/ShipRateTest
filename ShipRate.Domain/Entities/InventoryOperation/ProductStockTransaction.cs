using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class ProductStockTransaction : BusinessEntity
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int ProductTransactionTypeId { get; set; }
        public Guid? ProductRequestItemId { get; set; }
        public string Reference { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; }
        //Relations
        public Inventory Inventory { get; set; }
        public Product Product { get; set; }
        public ProductTransactionType ProductTransactionType { get; set; } // In / Out
        public ProductRequestItem ProductRequestItem { get; set; }
    }
}

/*باید کامل شود
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
