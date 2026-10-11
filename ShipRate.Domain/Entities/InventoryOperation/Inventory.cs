using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.InventoryOperation
{
    public class Inventory : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string InventoryCode { get; set; }
        public string InventoryName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        //Relations
        public Company Company { get; set; }
    }
}
