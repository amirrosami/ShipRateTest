using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Settings
{
    public class SystemSetting : SystemEntity
    {
        public int? CompanyId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //Relations
        public Company Company { get; set; }
    }
}
