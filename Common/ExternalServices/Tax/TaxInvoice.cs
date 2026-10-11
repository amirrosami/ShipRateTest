namespace ShipRate.Common.ExternalServices.Tax
{
    public class TaxInvoice
    {
        public Guid Id { get; set; }
        public TaxInvoiceStatus InvoiceStatus { get; set; }
        public string? taxid { get; set; }
        public long? indatim { get; set; }
        public long? indati2m { get; set; }
        public int? inty { get; set; }
        public string? inno { get; set; }
        public string? irtaxid { get; set; }
        public int? inp { get; set; }
        public int? ins { get; set; }
        public string? tins { get; set; }
        public int? tob { get; set; }
        public string? bid { get; set; }
        public string? tinb { get; set; }
        public string? sbc { get; set; }
        public string? bpc { get; set; }
        public string? bbc { get; set; }
        public int? ft { get; set; }
        public string? bpn { get; set; }
        public string? scln { get; set; }
        public string? scc { get; set; }
        public string? crn { get; set; }
        public string? billid { get; set; }
        public decimal? tprdis { get; set; }
        public decimal? tdis { get; set; }
        public decimal? tadis { get; set; }
        public decimal? tvam { get; set; }
        public decimal? todam { get; set; }
        public decimal? tbill { get; set; }
        public int? setm { get; set; }
        public decimal? cap { get; set; }
        public decimal? insp { get; set; }
        public decimal? tvop { get; set; }
        public decimal? tax17 { get; set; }


        public string? cdcn { get; set; }
        public int? cdcd { get; set; }
        public decimal? tonw { get; set; }
        public decimal? torv { get; set; }
        public decimal? tocv { get; set; }
        public string? tinc { get; set; }
        public string? lno { get; set; }
        public string? lrno { get; set; }
        public string? ocu { get; set; }
        public string? oci { get; set; }
        public string? dco { get; set; }
        public string? dci { get; set; }
        public string? tid { get; set; }
        public string? rid { get; set; }
        public string? lt { get; set; }
        public string? cno { get; set; }
        public string? did { get; set; }
        public string? asn { get; set; }
        public int? asd { get; set; }
        public string? In { get; set; }
        public string? an { get; set; }

        public List<TaxInvoiceShippingGood> sg = new List<TaxInvoiceShippingGood>();
        public List<TaxInvoiceDtl> body = new List<TaxInvoiceDtl>();
        public List<TaxInvoicePayment> payment = new List<TaxInvoicePayment>();
    }
}