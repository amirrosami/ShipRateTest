using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Settings
{
    public class CompanyContext : SystemEntity
    {
        public int CompanyId { get; set; }
        public int ContextTypeId { get; set; }
        public string CompanyContextCode { get; set; } = string.Empty;
        public string CompanyContextName { get; set; } = string.Empty;

        //Relations
        public ContextType ContextType { get; set; }
        public Company Company { get; set; }
    }
}
