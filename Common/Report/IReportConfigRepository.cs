namespace ShipRate.Common.Report
{
    public interface IReportConfigRepository
    {
        Task<ReportConfig> GetReportConfigAsync(string reportName);
    }
}
