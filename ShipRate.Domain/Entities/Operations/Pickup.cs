
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Pickup : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ShipmentId { get; set; }
        public DateTime RequestPickupDate { get; set; }
        public DateTime ActualPickupDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Remarks { get; set; }

        //Relations
        public Company Company { get; set; }
        public Shipment Shipment { get; set; }
    }
}
/*Bishtar Barrasi Shavad
 * Carrier?
 * Scenario....
 *خیلی چیزای دیگه . اگر بخواهیم در نظر بگیریم هست 
 * 
 */


