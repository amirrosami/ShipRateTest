using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BookingOperation;
namespace ShipRate.Application.Interfaces.ContainerActions
{
    public interface IContainerActionService
    {
        Task<ContainerActionIndexVm> SearchActionsAsync(ContainerActionSearchFormVm searchForm, int pageNumber, int pageSize);
        Task<ReleaseToShipperVm> PrepareReleaseToShipperVmAsync(ReleaseToShipperVm? model = null);
        Task ReleaseToShipperAsync(ReleaseToShipperVm model);
        Task<RtsDeliverySummaryVm> GetRtsDeliverySummaryAsync(string rtsNo);
        Task<RtsDeliverySummaryVm> GetRtsDeliverySummaryByInstructionIdAsync(Guid rtsInstructionId);
        Task<(bool Success, string Message)> IssueContainerAsync(IssueContainerCommand command, int companyId);
        Task<LoadOnBoardVm> PrepareLoadOnBoardVmAsync(LoadOnBoardVm? model = null);
        Task LoadOnBoardAsync(LoadOnBoardVm model);
        Task<DischargeVm> PrepareDischargeVmAsync(DischargeVm? model = null);
        Task DischargeAsync(DischargeVm model);
        Task DTCAsync(Guid doId);
        Task<RBCVm> PrepareRBCVmAsync(RBCVm? model = null);
        Task RBCAsync(RBCVm model);
        Task<bool> RegisterDischargeActionsForFinalPortVoyageAsync(Guid voyagePortId);
        Task<bool> RBSReceiptAsync(ExportDocsReceipt receipt);
    }
}
