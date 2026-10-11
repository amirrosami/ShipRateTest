using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class Driver : BusinessEntity
    {
        public Guid? CarrierId { get; set; }
        public string PassportNo { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string SmartCardNo { get; set; } = string.Empty;  //شماره کارت هوشمند

        //Relations

        public Carrier Carrier { get; set; }
        public Person Person { get; set; }
    }
}