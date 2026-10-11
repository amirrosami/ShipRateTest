using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Settings
{
    public class NumberingSystem : SystemEntity
    {
        public int CompanyId { get; set; }
        public string EntityType { get; set; }
        public string Prefix { get; set; }
        public string? Suffix { get; set; }
        public int PadLedngth { get; set; }
        public bool ResetYearly { get; set; }
        public int CurrentSeq { get; set; }
        public int CurrentYear { get; set; }
        //Relations
        public Company Company { get; set; }
    }
}