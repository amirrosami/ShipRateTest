using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class ProductTransactionType : SystemEntity
    {
        public string ProductTransactionTypeName { get; set; } = string.Empty;
        public string? ProductTransactionTypeLocalName { get; set; } = string.Empty;
        public int InOutId { get; set; }
        //Relation
        public InOut InOut { get; set; }
    }
}
