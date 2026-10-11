using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.Dashboard;
using ShipRate.Application.ViewModels.Dashboard;
using ShipRate.Infrastructure.Data;
using System.Globalization;

namespace ShipRate.Application.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {

        private readonly ApplicationDbContext _db;
        public DashboardService(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<List<MonthlyFinanceByCurrencyVm>> GetMonthlyFinanceByCurrencyAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.Now.Date;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            var monthlyInvoices = _db.Invoices
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.InvoiceDate.HasValue &&
                    x.InvoiceDate.Value >= currentMonthStart &&
                    x.InvoiceDate.Value < nextMonthStart &&
                    x.InvoiceStatusId != 99 &&

                    // فقط جهت‌های مالی موردنیاز داشبورد
                    (x.FinanceDirectionId == 1 ||
                     x.FinanceDirectionId == 2));

            var result = await monthlyInvoices
                .GroupBy(x => new
                {
                    x.CurrencyId,
                    CurrencyCode = x.Currency != null
                        ? x.Currency.CurrencyCode
                        : "N/A"
                })
                .Select(group => new MonthlyFinanceByCurrencyVm
                {
                    CurrencyId = group.Key.CurrencyId,
                    CurrencyCode = group.Key.CurrencyCode,

                    RevenueAmount = group
                        .Where(x => x.FinanceDirectionId == 1)
                        .Sum(x => (decimal?)x.TotalAmount) ?? 0m,

                    ExpenseAmount = group
                        .Where(x => x.FinanceDirectionId == 2)
                        .Sum(x => (decimal?)x.TotalAmount) ?? 0m
                })
                .OrderByDescending(x => x.RevenueAmount)
                .ToListAsync(cancellationToken);

            return result;
        }
        public async Task<List<MonthlyRevenueByCurrencyVm>> GetMonthlyRevenueByCurrencyAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now.Date;

            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);

            var invoicesQuery = _db.Invoices
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.InvoiceDate.HasValue &&
                    x.InvoiceStatusId != 99 &&

                    x.FinanceDirectionId == 1);

            var currentMonthRevenue = await invoicesQuery
                .Where(x =>
                    x.InvoiceDate!.Value >= currentMonthStart &&
                    x.InvoiceDate.Value < nextMonthStart)
                .GroupBy(x => new
                {
                    x.CurrencyId,
                    CurrencyCode = x.Currency != null
                        ? x.Currency.CurrencyCode
                        : "N/A"
                })
                .Select(group => new
                {
                    group.Key.CurrencyId,
                    group.Key.CurrencyCode,
                    Amount = group.Sum(x => x.TotalAmount)
                })
                .ToListAsync(cancellationToken);

            var previousMonthRevenue = await invoicesQuery
                .Where(x =>
                    x.InvoiceDate!.Value >= previousMonthStart &&
                    x.InvoiceDate.Value < currentMonthStart)
                .GroupBy(x => x.CurrencyId)
                .Select(group => new
                {
                    CurrencyId = group.Key,
                    Amount = group.Sum(x => x.TotalAmount)
                })
                .ToListAsync(cancellationToken);

            var previousAmounts = previousMonthRevenue
                .ToDictionary(x => x.CurrencyId, x => x.Amount);

            return currentMonthRevenue
                .Select(x => new MonthlyRevenueByCurrencyVm
                {
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.CurrencyCode ?? "N/A",
                    CurrentMonthAmount = x.Amount,
                    PreviousMonthAmount = previousAmounts.GetValueOrDefault(x.CurrencyId)
                })
                .OrderByDescending(x => x.CurrentMonthAmount)
                .ToList();
        }
        public async Task<CustomerGrowthChartVm> GetCustomerGrowthChartAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.Now.Date;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);

            // از پنج ماه قبل تا ابتدای ماه بعد
            var firstMonthStart = currentMonthStart.AddMonths(-5);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            var customerCreatedDates = await _db.Customers
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Person != null &&
                    !x.Person.IsDeleted &&
                    x.Person.CreatedAt >= firstMonthStart &&
                    x.Person.CreatedAt < nextMonthStart)
                .Select(x => x.Person.CreatedAt)
                .ToListAsync(cancellationToken);

            var labels = new List<string>();
            var counts = new List<int>();

            for (var i = 0; i < 6; i++)
            {
                var monthStart = firstMonthStart.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);

                labels.Add(monthStart.ToString("MMM yyyy", CultureInfo.InvariantCulture));

                var count = customerCreatedDates.Count(createdAt =>
                    createdAt >= monthStart &&
                    createdAt < monthEnd);

                counts.Add(count);
            }

            var currentMonthCount = counts[^1];
            var previousMonthCount = counts[^2];

            return new CustomerGrowthChartVm
            {
                Labels = labels,
                Counts = counts,
                TotalCustomers = counts.Sum(),
                CurrentMonthCount = currentMonthCount,
                PreviousMonthCount = previousMonthCount
            };
        }
    }
}
