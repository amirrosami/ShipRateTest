using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;
using ShipRate.Domain.Enums;


namespace ShipRate.Domain.Entities.Operations
{
    public class LongStayWarning : BusinessEntity
    {
        public int CompanyId { get; set; }
        public EnumInOut InOutId { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? CarrierId { get; set; }


        public Guid BLId { get; set; }
        public DateTime WarningDate { get; set; }
        public DateTime InventoryStartDate { get; set; }

        public int DeadLineDays { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Company Company { get; set; }
        public InOut InOut { get; set; }
        public Customer Customer { get; set; }
        public Carrier Carrier { get; set; }
        public BL BL { get; set; }
    }
}
