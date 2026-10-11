using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class LicenseStatus : SystemEntity
    {
        public string LicenseStatusName { get; set; }
        public string LicenseStatusLocalName { get; set; }
    }
}
