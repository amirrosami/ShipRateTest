namespace ShipRate.Common.Report
{
    public class ReportConfig
    {
        public Guid Id { get; set; }
        public string ReportName { get; set; }
        public string TemplatePath { get; set; }
        public bool Enabled { get; set; } = true;
        public string? ReportDscp { get; set; }
    }
}
