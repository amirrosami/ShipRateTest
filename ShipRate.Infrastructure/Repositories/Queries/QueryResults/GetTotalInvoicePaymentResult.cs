using System;
using System.Collections.Generic;
using System.Text;

namespace ShipRate.Infrastructure.Repositories.Queries.QueryResults
{
    public class GetRemainInvoicePaymentResult
    {
        public Guid InvoiceId { get; set; }
        public decimal? RemainPayment { get; set; }
    }


    public class GetTotalInvoicePaymentResult
    {
        public Guid InvoiceId { get; set; }
        public decimal? TotalPaid { get; set; }
    }


}
