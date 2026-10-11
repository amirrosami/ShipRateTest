using FastReport.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace ShipRate.Common.Report
{
    public class ReportService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IReportConfigRepository configRepository;
        public ReportService(IServiceProvider serviceProvider, IReportConfigRepository configRepository)
        {
            this.serviceProvider = serviceProvider;
            this.configRepository = configRepository;
        }

        private IReportExecutor<TInput, TResult> GetExecutor<TInput, TResult>()
        {
            return serviceProvider.GetRequiredService<IReportExecutor<TInput, TResult>>();
        }

        private IReportExecutor<TInput> GetExecutor<TInput>()
            where TInput : ReportRequest
        {
            return serviceProvider.GetRequiredService<IReportExecutor<TInput>>();
        }

        public async Task<ReportViewModel> ViewReport<TInput>(TInput input)
            where TInput : ReportRequest
        {
            var reportConfig = await configRepository.GetReportConfigAsync(input.ReportName);
            if (reportConfig == null)
            {
                throw new Exception("Report with this name doesnt exists.");
            }
            if (!reportConfig.Enabled)
            {
                throw new Exception("This Report is Disable!!");
            }
            var report = new WebReport();
            report.Toolbar.Exports = new ExportMenuSettings()
            {
                ExportTypes = Exports.Excel2007
            };
            report.Toolbar.Exports = ExportMenuSettings.All;
            report.Toolbar.Exports.EnableSettings = true;

            var reportExecutor = GetExecutor<TInput>();
            IEnumerable data = await reportExecutor.ExecuteAsync(input);
            report.Report.RegisterData(data, input.ReportName);
            report.Report.Load(reportConfig.TemplatePath);
            var isPrepared = await report.Report.PrepareAsync();
            if (!isPrepared)
            {
                throw new Exception($"Error in Report Preparing!");
            }
            return new ReportViewModel
            {
                Viewer = report,
                ReportName = input.ReportName
            };
        }


        public async Task Export(ExportType type)
        {

        }

    }
}
