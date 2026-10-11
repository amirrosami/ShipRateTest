using ShipRate.Application.ViewModels.Dashboard;

namespace ShipRate.Application.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<List<MonthlyFinanceByCurrencyVm>> GetMonthlyFinanceByCurrencyAsync(int companyId, CancellationToken cancellationToken = default);
        Task<List<MonthlyRevenueByCurrencyVm>> GetMonthlyRevenueByCurrencyAsync(int companyId, CancellationToken cancellationToken = default);
        Task<CustomerGrowthChartVm> GetCustomerGrowthChartAsync(CancellationToken cancellationToken = default);

    }
}
