namespace ShipRate.Common.ExternalServices.Tax
{
    public class TaxInvoiceDtl
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string? sstid { get; set; }
        public string? sstt { get; set; }
        public string? mu { get; set; }
        public double? am { get; set; }
        public decimal? fee { get; set; }
        public decimal? cfee { get; set; }
        public string? cut { get; set; }
        public decimal? exr { get; set; }
        public decimal? prdis { get; set; }
        public decimal? dis { get; set; }
        public decimal? adis { get; set; }
        public decimal? vra { get; set; }
        public decimal? vam { get; set; }
        public string? odt { get; set; }
        public decimal? odr { get; set; }
        public decimal? odam { get; set; }
        public string? olt { get; set; }
        public decimal? olr { get; set; }
        public decimal? olam { get; set; }
        public decimal? consfee { get; set; }
        public decimal? spro { get; set; }
        public decimal? bros { get; set; }
        public decimal? tcpbs { get; set; }
        public decimal? cop { get; set; }
        public decimal? vop { get; set; }
        public string? bsrn { get; set; }
        public decimal? tsstam { get; set; }
        public decimal? nw { get; set; }
        public decimal? ssrv { get; set; }
        public decimal? sscv { get; set; }
        public decimal? cui { get; set; }
        public decimal? hs { get; set; }
        public decimal? vba { get; set; }
        public decimal? cpr { get; set; }
        public long? sovat { get; set; }
        public TaxInvoice Invoice { get; set; }
    }
}