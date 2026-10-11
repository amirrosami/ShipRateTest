using ShipRate.Application.DTOs.Reports;
using ShipRate.Common.Report;
using System.Collections;

namespace ShipRate.Application.Services.Reports
{

    public class TestReport : IReportExecutor<TestReportRequest>
    {
        public async Task<IEnumerable> ExecuteAsync(TestReportRequest input)
        {
            List<object> results = new List<object>();
            for (int i = 0; i < 30; i++)
            {
                results.Add(
                    new { Country = $"country{i % 10}", City = $"City{i}" });
            }

            return (IEnumerable)results;
        }
    }
}
