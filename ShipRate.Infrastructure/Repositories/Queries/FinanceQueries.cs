using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Repositories.Queries.QueryResults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShipRate.Infrastructure.Repositories.Queries
{
    public class FinanceQueries
    {
        private readonly ApplicationDbContext _db;

        public FinanceQueries(ApplicationDbContext db)
        {
            _db = db;
        }

        public IQueryable<GetTotalInvoicePaymentResult> GetTotalInvoicePayment() {
            return _db.SettlementItems.GroupBy(k => k.InvoiceId).Select(i => new GetTotalInvoicePaymentResult
            {
                InvoiceId = i.Key.Value,
                TotalPaid = i.Sum(x => x.Amount)
            });
        }

        public IQueryable<GetRemainInvoicePaymentResult> GetRemainInvoicePayment()
        {
            return _db.Invoices.Include(x=>x.SettlementItems).Select(i => new GetRemainInvoicePaymentResult
            {
                InvoiceId = i.Id,
                RemainPayment = i.TotalAmount - i.SettlementItems.Sum(s=>s.Amount)
            });
        }

        //public async Task<List<GetRemainInvoicePaymentResult>> GetRemainInvoicesPayment(List<Guid> invoiceIds)
        //{

        //    var invoicesStr = string.Join(",",invoiceIds.Select(x => "'" + x + "'"));
        //    return await _db.Database.SqlQuery<List<GetRemainInvoicePaymentResult>>(
        //        $"""
        //        SELECT InvoiceId , TotalAmount - Sum(PaymentAmount) As Payment
        //        From fin.OnlinePaymentItems as items 
        //        Inner Join OnlinePayments as o on o.Id == items.OnlinePaymentId
        //        Inner Join fin.Invoices as i on i.Id == items.InvoiceId
        //        WHERE InvoiceId in ({invoicesStr}))

        //        """



        //        );
        //}


    }
}
