using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Truck : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int TruckTypeId { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? LastDriverId { get; set; }
        public string TruckReferenceTitle { get; set; } = string.Empty;  //عنوان جهت ارجاع بهتر
        public string TruckPlateNumber { get; set; } = string.Empty;  //شماره شهربانی
        public string TrailerPlateNumber { get; set; } = string.Empty;  //پلاک ترانزیت کانتین
        public decimal? MaxPayloadKg { get; set; }
        public decimal? MaxVolumeCBM { get; set; }
        public int CountryId { get; set; }
        public string FleetSmartCardNo { get; set; } = string.Empty; //شماره کارت هوشمند ناوگان

        public string TPInsuranceNo { get; set; }   //شماره بیمه شخص ثالث
        public DateTime TPInsuranceExpiryDate { get; set; }   //تاریخ انقضای بیمه شخص ثالث


        public int? AxleCount { get; set; }   //تعداد محور
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? YearManufactured { get; set; }



        //Relations
        public Carrier? Carrier { get; set; }
        public Person Owner { get; set; }
        public Person LastDriver { get; set; }
        public TruckType TruckType { get; set; }
        public Country Country { get; set; }
        public Company Company { get; set; }
    }
}


/*Driver
 *Person = Owner
 * 
 * 
 * 
 */