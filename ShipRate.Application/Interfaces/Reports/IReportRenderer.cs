using FastReport;

namespace ShipRate.Application.Interfaces.Reports
{
    public interface IReportRenderer
    {
        Task<byte[]> ExportPdfAsync<TReportVm>(
            string templateRelativePath,
            TReportVm model,
            Action<Report, TReportVm>? registerData = null,
            CancellationToken cancellationToken = default);

        Task<Report> CreatePreparedReportAsync<TReportVm>(
            string templateRelativePath,
            TReportVm model,
            Action<Report, TReportVm>? registerData = null,
            CancellationToken cancellationToken = default);
    }
}
