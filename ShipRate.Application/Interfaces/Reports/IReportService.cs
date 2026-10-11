using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Reports
{
    public interface IReportService
    {
        Task<ReportsIndexViewModel> GetAuthorizedReportsAsync(int companyId);
        Task<ReportsIndexViewModel> GetAllReportsAsync();
        Task<List<Rpt_ContainerDto>> GetContainersAsync();
        Task<RateRequestReportDto?> GetRateRequestReportAsync(Guid? rateRequestId);
        Task<RateRequestReportDto?> GetRateRequestReportsAsync();    //فعلا برای سادگی فقط یکی
        Task<RTSInstructionDetailsVm> GetRTSInstructionLetterAsync(Guid id);
        //Task<byte[]> PrintRTSInstructionLetterPdfAsync(Guid id);
        Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId);
    }
}
