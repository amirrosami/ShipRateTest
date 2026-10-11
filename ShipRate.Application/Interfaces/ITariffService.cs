using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface ITariffService
    {
        Task<TariffIndexVm> GetIndexVmAsync(TariffBookSearchVm search, CancellationToken cancellationToken = default);

        Task<PagedResultDto<TariffBookListItemVm>> GetTariffBooksAsync(
            TariffBookSearchVm search,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<TariffRuleListItemVm>> GetTariffRulesAsync(
            Guid tariffBookId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<TariffRuleCoverageListItemVm>> GetTariffRuleCoveragesAsync(
            Guid tariffRuleId,
            CancellationToken cancellationToken = default);

        Task<bool> RuleCanShowCoveragesAsync(
            Guid tariffRuleId,
            CancellationToken cancellationToken = default);

        Task<TariffBookFormVm> GetCreateTariffBookVmAsync(CancellationToken cancellationToken = default);

        Task<TariffBookFormVm?> GetEditTariffBookVmAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateTariffBookAsync(
            TariffBookFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateTariffBookAsync(
            TariffBookFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> SoftDeleteTariffBookAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<TariffRuleFormVm> GetCreateTariffRuleVmAsync(
            int tariffTyppeId,
            Guid tariffBookId,
            CancellationToken cancellationToken = default);

        Task<TariffRuleFormVm?> GetEditTariffRuleVmAsync(
            int tariffTypeId,
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateTariffRuleAsync(
            TariffRuleFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateTariffRuleAsync(
            TariffRuleFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> SoftDeleteTariffRuleAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<TariffRuleCoverageFormVm> GetCreateCoverageVmAsync(
            Guid tariffRuleId,
            CancellationToken cancellationToken = default);

        Task<TariffRuleCoverageFormVm?> GetEditCoverageVmAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateCoverageAsync(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateCoverageAsync(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken = default);

        Task<bool> SoftDeleteCoverageAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<PartyLookupItemVm>> SearchPartiesAsync(
            string? term,
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetTariffTypeSelectListAsync(CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetChargeTypeSelectListAsync(int TariffTypeId, CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetTariffStatusSelectListAsync(CancellationToken cancellationToken = default);
        Task ReplaceDemurrageRulesAsync(DemurrageRuleBatchFormVm vm);
        Task<DemurrageRuleBatchFormVm> BuildDemurrageRuleBatchFormAsync(Guid tariffBookId, int? containerSizeTypeId = null);
        Task<List<SelectListItem>> GetContainerSizeTypesListAsync(CancellationToken cancellationToken = default);

        Task<int> GetTariffTypeIdAsync(Guid tariffBookId, CancellationToken cancellationToken = default);
        Task<string> GetTariffTitleAsync(Guid tariffBookId, CancellationToken cancellationToken);

        Task PopulateOceanFreightRuleBatchLookupsAsync(OceanFreightRuleBatchFormVm model);


        Task<OceanFreightRuleBatchFormVm> BuildOceanFreightRuleBatchFormAsync(
            Guid tariffBookId,
            Guid? originId = null,
            Guid? destinationId = null,
            Guid? partyId = null,
            OceanFreightRuleBatchFormVm? postedModel = null);

        Task<OceanFreightRuleBatchFormVm> AddOceanFreightRuleRowAsync(
            OceanFreightRuleBatchFormVm model);

        Task<OceanFreightRuleBatchFormVm> RemoveOceanFreightRuleRowAsync(
            OceanFreightRuleBatchFormVm model,
            int rowIndex);

        Task<OceanFreightRuleBatchFormVm> PasteOceanFreightRuleRowsAsync(
            OceanFreightRuleBatchFormVm model);

        Task<ServiceResult> SaveOceanFreightRulesAsync(
            OceanFreightRuleBatchFormVm model,
            Guid currentUserId);
        Task<OceanFreightRuleBatchFormVm> LoadOceanFreightRulesForHeaderAsync(OceanFreightRuleBatchFormVm model);
        Task<bool> ActivateTariffBookAsync(Guid tariffBookId);




        // ============================================================
        // متدهای اختصاصی THC (TariffTypeId = 4)
        // ============================================================
        Task<TariffThcRuleBatchFormVm> BuildThcRuleBatchFormAsync(Guid tariffBookId, CancellationToken cancellationToken = default);
        Task<TariffThcRuleBatchFormVm> RebuildThcRuleBatchFormAsync(TariffThcRuleBatchFormVm model, CancellationToken cancellationToken = default);
        List<TariffThcRuleRowVm> ParseThcRulePastedText(string? pastedText);
        Task<bool> SaveThcRulesAsync(TariffThcRuleBatchFormVm model, CancellationToken cancellationToken = default);
        Task<List<LocationLookupVm>> SearchLocationsAsync(string? term, CancellationToken cancellationToken = default);
        Task<List<SelectListItem>> GetTradeTypesSelectListAsync(CancellationToken cancellationToken = default);
        Task<TariffRuleForSaoiVM> BuildLocalCntSaoiFormAsync(Guid tariffBookId, CancellationToken cancellationToken = default);
        Task<bool> SaveTariffRuleForSaoiAsync(TariffRuleForSaoiVM tariffRule, CancellationToken cancellationToken);
        Task<List<SelectListItem>> GetFullMTsListAsync(CancellationToken cancellationToken);
    }


    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; } = new();

        public static ServiceResult Success(string? message = null)
        {
            return new ServiceResult
            {
                Succeeded = true,
                Message = message
            };
        }

        public static ServiceResult Failure(string key, string message)
        {
            var result = new ServiceResult
            {
                Succeeded = false
            };

            result.Errors[key] = new List<string> { message };
            return result;
        }

        public void AddError(string key, string message)
        {
            Succeeded = false;

            if (!Errors.ContainsKey(key))
                Errors[key] = new List<string>();

            Errors[key].Add(message);
        }
    }



}
