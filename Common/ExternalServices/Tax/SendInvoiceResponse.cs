namespace ShipRate.Common.ExternalServices.Tax
{
    public class SendInvoiceResponse
    {
        public string Id { get; set; }
        public TaxInvoiceStatus InvoiceStatus { get; set; }
        public string ReferenceNumber { get; set; }
        public string TaxId { get; set; }
        public object? Data { get; set; }
    }
}


