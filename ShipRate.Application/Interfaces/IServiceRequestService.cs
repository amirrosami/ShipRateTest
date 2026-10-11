using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceRequestIndexDto> GetIndexDataAsync(ServiceRequestSearchDto input);
        Task<ServiceRequestFormDto> GetCreateFormAsync();
        Task<ServiceRequestFormDto> GetEditFormAsync(Guid id);
        Task<Guid> CreateAsync(ServiceRequestCreateUpdateDto input);
        Task UpdateAsync(ServiceRequestCreateUpdateDto input);
        Task DeleteAsync(Guid id);
        Task<List<SPersonLookupDto>> SearchPersonsAsync(string? term, int take = 10);



        //Approvals
        Task<ServiceRequestApprovalsPanelDto> GetApprovalsPanelAsync(Guid serviceRequestId);
        Task<ServiceRequestApprovalFormDto> GetApprovalCreateFormAsync(Guid serviceRequestId);
        Task<ServiceRequestApprovalFormDto> GetApprovalEditFormAsync(Guid id);
        Task<Guid> SaveApprovalAsync(ServiceRequestApprovalFormDto dto);
        Task DeleteApprovalAsync(Guid id);


        //Charges
        Task<ServiceRequestChargesPanelDto> GetChargesPanelAsync(Guid serviceRequestId);
        Task<ServiceRequestChargeFormDto> GetCreateChargeFormAsync(Guid serviceRequestId);
        Task<ServiceRequestChargeFormDto?> GetEditChargeFormAsync(Guid chargeId);
        Task SaveChargeAsync(ServiceRequestChargeFormDto model);
        Task DeleteChargeAsync(Guid id);
        Task<List<SelectListItem>> GetChargeTypeSelectListAsync();
        Task<List<SelectListItem>> GetCurrencySelectListAsync();


        //BL Amendments
        Task<BLAmendmentsPanelDto> GetBLAmendmentsPanelAsync(Guid serviceRequestId);

        Task<BLAmendmentRequestFormDto> GetCreateBLAmendmentRequestFormAsync(Guid serviceRequestId);
        Task<BLAmendmentRequestFormDto?> GetEditBLAmendmentRequestFormAsync(Guid id);
        Task SaveBLAmendmentRequestAsync(BLAmendmentRequestFormDto model);
        Task DeleteBLAmendmentRequestAsync(Guid id);
        Task<BLAmendmentRequestLineFormDto> GetCreateBLAmendmentRequestLineFormAsync(Guid blAmendmentRequestId);
        Task<BLAmendmentRequestLineFormDto?> GetEditBLAmendmentRequestLineFormAsync(Guid id);
        Task SaveBLAmendmentRequestLineAsync(BLAmendmentRequestLineFormDto model);
        Task DeleteBLAmendmentRequestLineAsync(Guid id);
        Task<List<SelectListItem>> GetBLSelectListAsync(Guid serviceRequestId);
        Task<List<SelectListItem>> GetBLAmendmentFieldTypeSelectListAsync();
        Task<BLAmendmentFullRequestFormDto> GetCreateBLAmendmentFullRequestFormAsync();
        Task<BLAmendmentFullRequestFormDto> RebuildBLAmendmentFullRequestFormAsync(BLAmendmentFullRequestFormDto model);
        Task<ServiceResult> SaveBLAmendmentFullRequestAsync(BLAmendmentFullRequestFormDto model);
        Task<BLAmendmentLineItemViewDto> GetBlankBLAmendmentLineAsync(int index);



        //Split
        Task<SplitRequestPanelDto?> GetSplitRequestPanelAsync(Guid serviceRequestId);
        Task<AdvancedBLSplitVm?> GetAdvancedSplitModelAsync(Guid splitRequestId);
        Task<CreateSplitRequestResult> CreateSplitRequestAsync(CreateSplitRequestVm model);
        Task<SubmitAdvancedSplitResult> SubmitAdvancedSplitAsync(SubmitAdvancedSplitVm model);
    }




    public class CreateSplitRequestResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? SplitRequestId { get; set; }

        public static CreateSplitRequestResult Ok(Guid splitRequestId) => new()
        {
            Success = true,
            StatusCode = 200,
            SplitRequestId = splitRequestId
        };

        public static CreateSplitRequestResult Fail(int statusCode, string message) => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
    }





}
