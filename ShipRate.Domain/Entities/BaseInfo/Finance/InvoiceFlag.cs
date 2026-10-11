using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class InvoiceFlag : SystemEntity
    {

        public string InvoiceFlagDescription { get; set; } = string.Empty;
    }
}