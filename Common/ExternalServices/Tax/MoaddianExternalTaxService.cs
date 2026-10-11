
namespace ShipRate.Common.ExternalServices.Tax
{
    public class MoaddianExternalTaxService : ExternalTaxService
    {
        public override Task<TaxInvoiceStatus> GetInvoiceResultAsync(string id)
        {
            throw new NotImplementedException();
        }

        public override Task<SendInvoiceResponse> SendInvoiceAsync(TaxInvoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}
