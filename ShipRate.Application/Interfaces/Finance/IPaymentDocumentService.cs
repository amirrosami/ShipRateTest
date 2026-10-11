using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface IPaymentDocumentService
    {
        Task<PagedResultDto<PaymentDocumentListItemVm>> GetPagedAsync(PaymentDocumentSearchVm searchModel);
        Task<PaymentDocumentCreateEditVm> GetCreateModelAsync();
        Task<PaymentDocumentCreateEditVm?> GetEditModelAsync(Guid id);
        Task<Guid> CreateAsync(PaymentDocumentCreateEditVm model, string? currentUserId = null);
        Task<bool> UpdateAsync(PaymentDocumentCreateEditVm model);
        Task<bool> SoftDeleteAsync(Guid id, string? currentUserId = null);
        Task<PaymentDocumentDetailsVm?> GetDetailsAsync(Guid id);
        Task<List<PersonLookupVm>> SearchPersonsAsync(string? term, int take = 10);
        Task<List<SelectListItem>> GetBankAccountsByBankIdAsync(Guid? bankId);
        Task<IEnumerable<PaymentDocumentListItemVm>> GetEligibleAsync(
                int companyId,
                Guid partyId,
                int treasuryDocumentTypeId,
                CancellationToken cancellationToken);
        Task<List<AvailablePaymentDocDto>> GetAvailablePaymentDocsByPartyAsync(Guid partyId);
        Task<List<AvailablePaymentDocDto>> GetAvailablePaymentDocumentsAsync(Guid partyId, int? currencyId = null, int? companyId = null, CancellationToken cancellationToken = default);
        Task<bool> CanBeDeletedPaymentDocumentAsync(Guid id);
        Task<PaymentDocumentUsagePanelVm?> GetUsagePanelAsync(Guid paymentDocumentId, CancellationToken cancellationToken = default);
        Task<List<PaymentDocumentLookupDto>> GetAvailablePaymentDocumentsForVaultAsync();
    }
}
