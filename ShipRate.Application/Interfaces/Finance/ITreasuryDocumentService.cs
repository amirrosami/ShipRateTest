using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using PersonLookupDto = ShipRate.Application.DTOs.Finance.PersonLookupDto;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface ITreasuryDocumentService
    {
        Task<PagedResultDto<TreasuryDocumentListItemDto>> SearchAsync(TreasuryDocumentSearchDto input);

        Task<TreasuryDocumentFormDto> GetCreateModelAsync();
        Task<TreasuryDocumentFormDto?> GetEditModelAsync(Guid id);

        Task<List<PersonLookupDto>> SearchPersonsAsync(string term);

        Task<List<TreasuryDocumentPaymentSelectorDto>> GetAvailablePaymentDocumentsByPartyAsync(
            Guid partyId,
            Guid? treasuryDocumentId = null);

        Task<Guid> CreateAsync(TreasuryDocumentSaveDto input, Guid currentUserId);
        Task UpdateAsync(TreasuryDocumentSaveDto input, Guid currentUserId);

        Task<List<SelectListItem>> GetCompanySelectListAsync();
        Task<List<SelectListItem>> GetCurrencySelectListAsync();
        Task<List<SelectListItem>> GetTreasuryDocumentTypeSelectListAsync();
        Task<List<SelectListItem>> GetTreasuryDocumentStatusSelectListAsync();
        Task<(bool Success, string Message)> DeleteAsync(Guid id);
        Task ConfirmAsync(Guid id, Guid currentUserId);
    }
}
