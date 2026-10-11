
using Microsoft.EntityFrameworkCore;
using ShipRate.Common.ExternalServices.Tax;
using ShipRate.Common.Report;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class ReportConfigRepository : IReportConfigRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ReportConfigRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ReportConfig> GetReportConfigAsync(string reportName)
        {
            return await dbContext.ReportConfigs.Where(x => x.ReportName == reportName).FirstOrDefaultAsync();
        }
    }
    public class TaxRepository : ITaxRepository
    {
        private readonly ApplicationDbContext _context;

        public TaxRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateInvoiceAsync(TaxInvoice invoice)
        {
            _context.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(Guid invoiceId)
        {
            var invoice = await _context.TaxInvoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.TaxInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
            throw new Exception("Invoice doesnt Exists!");
        }

        public async Task<TaxInvoice> GetInvoiceAsync(Guid invoiceId)
        {
            return await _context.TaxInvoices.FindAsync(invoiceId);
        }

        public async Task UpdateInvoiceAsync(TaxInvoice invoice)
        {
            _context.TaxInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
