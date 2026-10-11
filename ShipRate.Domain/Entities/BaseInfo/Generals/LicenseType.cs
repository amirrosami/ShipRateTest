using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class LicenseType : SystemEntity
    {
        public string LicenseTypeName { get; set; }
        public string LicenseTypeLocalName { get; set; }
        public string IssuingOrgName { get; set; }
    }
}
