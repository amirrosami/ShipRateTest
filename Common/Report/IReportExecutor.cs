using System.Collections;

namespace ShipRate.Common.Report
{
    public interface IReportExecutor<TInput>
    {
        Task<IEnumerable> ExecuteAsync(TInput input);
    }

    public interface IReportExecutor<TInput, TResult>
    {
        Task<IEnumerable<TResult>> ExecuteAsync(TInput input);
    }
}
