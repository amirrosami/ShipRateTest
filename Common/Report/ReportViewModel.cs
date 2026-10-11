using FastReport.Web;

namespace ShipRate.Common.Report
{

    public class ReportViewModel<TViewer>
    {
        public TViewer Viewer { get; set; }
        public string ReportName { get; set; }
    }

    public class ReportViewModel : ReportViewModel<WebReport> { }

}
