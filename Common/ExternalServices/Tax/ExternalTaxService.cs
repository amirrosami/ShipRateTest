namespace ShipRate.Common.ExternalServices.Tax
{
    public abstract class ExternalTaxService
    {
        public abstract Task<SendInvoiceResponse> SendInvoiceAsync(TaxInvoice invoice);
        public abstract Task<TaxInvoiceStatus> GetInvoiceResultAsync(string id);
    }
}


