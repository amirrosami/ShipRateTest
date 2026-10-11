using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.ViewModels;
using ShipRate.Application.ViewModels.Dashboard;

namespace ShipRate.Application.Interfaces.Jobs
{
    public interface IJobService
    {
        //Jobs
        Task<PagedResultDto<JobListItemVm>> SearchAsync(JobSearchVm search);
        Task<CreateJobVm> BuildCreateVmAsync();
        Task<CreateJobVm> BuildEditVmAsync(Guid id);
        Task<Guid> CreateAsync(CreateJobVm vm);
        Task<bool> UpdateAsync(CreateJobVm vm);
        Task<JobStatsViewModel> Stats();
        Task<JobDetailsVm?> GetDetailsAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<(bool CanDelete, string Message)> CanBeDeletedAsync(Guid id);
        //Task<List<JobTimelineItemVm>> GetHistoryTabAsync(Guid id);

        Task<List<PartyLookupItemVm>> SearchPartiesAsync(string? term);
        Task<List<LocationLookupItemVm>> SearchLocationsAsync(string? term);
        Task<List<HSCodeLookupItemVm>> SearchHSCodesAsync(string? term);
        Task<List<PackageTypeLookupItemVm>> SearchPackageTypesAsync(string? term);

        Task<List<RateRequestContainerListItemVm?>> GetContainersListAsync(Guid id);
        Task<Guid> GenerateJobFromAcceptedSellQuoteAsync(Guid quoteId);
        Task<bool> JobExistsToConfirmAsync(Guid myJobId);
        Task<bool> OperationConfirmAsync(Guid myJobId);
        Task<string> GetJobFirstShipmentTitleAsync(Guid jobid);
        string GenerateNewJobNumberAsync();
        Task<Guid> GetJobIdForQuickSearchTermAsync(string searchTerm);
        Task<JobNoFileNoVm> GetJobNoFileNumberAsync(Guid result);

        //Charges
        Task<JobChargesVm> GetChargesVmAsync(Guid id);
        Task CreateChargeAsync(CreateChargeVm vm);
        Task<CreateChargeVm> GetChargeDetailsAsync(Guid chargeId);
        Task<ChargeDeleteVm?> BuildChargeDeleteModelAsync(Guid id);
        Task<bool> DeleteChargeAsync(Guid id);




        //Shipments
        Task<Guid> GetFirstShipmentIdAsync(Guid jobId);


        //Bookings
        Task<BookingTabVm> GetBookingTabAsync(Guid id);
        Task<CreateBookingVm> BuildCreateBookingVmAsync();
        //Task<CreateBookingVm> BuildEditBookingVmAsync(Guid id);
        Task CreateBookingAsync(CreateBookingVm vm);
        Task UpdateBookingAsync(CreateBookingVm vm);
        Task<bool> DeleteBookingAsync(Guid id);
        Task<Guid> GetBookingJobIdAsync(Guid bookingId);
        Task<CreateBookingVm> GetBookingViewModelAsync(Guid jobId, Guid? bookingId = null, int? transportTypeId = null, int? companyRoleTypeId = null);
        Task<List<SelectListItem>> GetJobPartiesSelectListByBookingIdAsync(Guid bookingId);



        //RTSInstructions
        Task<List<RTSInstructionListItemVm>> GetRTSInstructionsByBookingIdAsync(Guid bookingId);
        Task AddRTSInstructionAsync(CreateRTSInstructionVm model);
        Task UpdateRTSInstructionAsync(CreateRTSInstructionVm model);
        Task<List<RTSContainerInputVm>> GetBookingContainerTypesToCreateRTSIAsync(Guid bookingId);
        Task<RTSInstructionDetailsVm> GetRTSInstructionDetailsAsync(Guid id);
        Task<CreateRTSInstructionVm> GetRTSInstructionForEditAsync(Guid bookingId);
        Task<List<RTSContainerInputVm>> GetBookingContainerTypesForEditRTSIAsync(Guid rtsiId, Guid bookingId);
        Task<bool> DeleteRTSInstructionAsync(Guid id);
        Task<bool> ConfirmAndIssueRTSInstructionAsync(Guid id);




        //BLs
        Task<List<BLListItemVm>> GetBLTabAsync(Guid id);
        Task<CreateBLVm> BuildCreateBLVmAsync();
        Task<CreateBLVm> BuildEditBLVmAsync(Guid id);
        Task CreateBLAsync(CreateBLVm vm);
        Task UpdateBLAsync(CreateBLVm vm);
        Task<List<SelectListItem>> GetJobPartiesSelectListByBLIdAsync(Guid blId);



        //Dashboard
        Task<int> GetActiveShipmentsAsync();
        Task<List<TransportationModeChartItemVm>> GetTransportationModeChartAsync(int companyId, CancellationToken cancellationToken = default);
        Task<ShipmentVolumeChartVm> GetShipmentVolumeChartAsync(int companyId, string range, CancellationToken cancellationToken = default);
        Task<List<TopShippingRouteVm>> GetTopShippingRoutesAsync(int companyId, int take = 5, CancellationToken cancellationToken = default);
    }
}
