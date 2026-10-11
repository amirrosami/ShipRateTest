namespace ShipRate.Common.ExternalServices.Tax
{
    public class TaxService
    {
        private readonly ExternalTaxService externalTaxService;
        private readonly ITaxRepository taxRepository;
        public TaxService(ExternalTaxService externalTaxService, ITaxRepository taxRepository)
        {
            this.externalTaxService = externalTaxService;
            this.taxRepository = taxRepository;
        }

        public async Task SendInvoiceAsync(Guid invoiceId)
        {
            TaxInvoice invoice = await taxRepository.GetInvoiceAsync(invoiceId);
            if (invoice == null)
            {
                throw new Exception($"Not found Invoice with id = {invoiceId}");
            }
            if (invoice.InvoiceStatus == TaxInvoiceStatus.SentSuccessfully)
            {
                throw new Exception($"Invoice {invoice.Id} has already been sent.");
            }
            var response = await externalTaxService.SendInvoiceAsync(invoice);
            if (response == null || response.InvoiceStatus == TaxInvoiceStatus.FailedToSend)
            {
                invoice.InvoiceStatus = TaxInvoiceStatus.FailedToSend;
                await taxRepository.UpdateInvoiceAsync(invoice);
                throw new Exception("invoice failed to send!");
            }
            invoice.InvoiceStatus = response.InvoiceStatus;
            await taxRepository.UpdateInvoiceAsync(invoice);
        }

        public async Task<TaxInvoiceStatus> GetInvoiceResultAsync(Guid invoiceId)
        {
            TaxInvoice invoice = await taxRepository.GetInvoiceAsync(invoiceId);
            if (invoice == null)
            {
                throw new Exception($"Not found Invoice with id = {invoiceId}");
            }
            TaxInvoiceStatus status = await externalTaxService.GetInvoiceResultAsync(invoice.taxid);
            return status;
        }

        public async Task CreateInvoiceAsync(TaxInvoice invoice)
        {
            var errors = ValidateInvoice(invoice);
            if (errors != null && errors.Count > 0)
            {
                throw new Exception($"{string.Join("\n", errors)}");
            }
            invoice.InvoiceStatus = TaxInvoiceStatus.NotSent;
            await taxRepository.CreateInvoiceAsync(invoice);
        }

        public async Task DeleteInvoiceAsync(Guid invoiceId) { }
        public async Task UpdateInvoiceAsync(TaxInvoice invoice) { }

        private List<string> ValidateInvoice(TaxInvoice invoice)
        {
            return new List<string>();
        }
    }
}
