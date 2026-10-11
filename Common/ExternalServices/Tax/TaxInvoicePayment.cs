namespace ShipRate.Common.ExternalServices.Tax
{
    public class TaxInvoicePayment
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string? iinn { get; set; }
        public string? acn { get; set; }
        public string? trmn { get; set; }
        public string? trn { get; set; }
        public string? pcn { get; set; }
        public string? pid { get; set; }
        public long? pdt { get; set; }
        public int? pmt { get; set; }
        public long? pv { get; set; }
        public string? vatpid { get; set; }
        public string? vatbid { get; set; }
        public TaxInvoice Invoice { get; set; }

    }
}