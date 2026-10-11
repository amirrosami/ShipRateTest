namespace ShipRate.Common.ExternalServices.Tax
{
    public interface ITaxRepository
    {
        Task<TaxInvoice> GetInvoiceAsync(Guid invoiceId);
        Task UpdateInvoiceAsync(TaxInvoice invoice);
        Task DeleteInvoiceAsync(Guid invoiceId);
        Task CreateInvoiceAsync(TaxInvoice invoice);
    }
}
