namespace ShipRate.Common.ExternalServices.Tax
{
    public class TaxInvoiceShippingGood
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string SgId { get; set; }
        public string Sgt { get; set; }
        public TaxInvoice Invoice { get; set; }

    }
}