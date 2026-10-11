using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using LookupDto = ShipRate.Application.DTOs.Finance.LookupDto;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface ISettlementService
    {
        Task<PagedResultDto<SettlementListItemDto>> SearchAsync(SettlementSearchDto input);
        Task<SettlementHeaderCreateDto> GetCreateModelAsync();
        Task<Guid> CreateHeaderAsync(SettlementHeaderCreateDto input, Guid userId);
        Task<SettlementDetailsDto?> GetDetailsAsync(Guid id);
        Task<SettlementHeaderEditDto?> GetHeaderForEditAsync(Guid id);
        Task UpdateHeaderAsync(SettlementHeaderEditDto input, Guid userId);
        Task<List<SettlementItemDto>> GetItemsAsync(Guid settlementId);

        Task<Guid> AddItemAsync(SettlementItemCreateDto input, Guid userId);

        Task DeleteItemAsync(Guid id);
        Task<List<PaymentAllocationDto>> GetPaymentAllocationsAsync(Guid settlementId);
        Task AddPaymentAllocationAsync(PaymentAllocationCreateDto input, Guid userId);
        Task DeletePaymentAllocationAsync(Guid id);
        Task<List<LookupDto>> SearchPartiesAsync(string term);
        Task<List<LookupDto>> SearchInvoicesAsync(Guid? partyId, string term);
        Task<List<LookupDto>> SearchPaymentDocumentsAsync(Guid? partyId, string term);
        Task<List<LookupDto>> SearchChargesAsync(Guid? partyId, string term);
        Task<PaymentAllocationFormDto> GetPaymentAllocationForm(Guid settlementId);
        Task<List<JobSettlementGridRowDto>> GetJobSettlementsAsync(Guid jobId);
        Task<List<JobSettlementDetailDto>> GetSettlementItemDetailsAsync(Guid settlementItemId);
        Task<DTOs.Finance.OperationResult> ApproveSettlementAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<SettlementReceiptReportDto?> GetSettlementReceiptReportDataAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<InvoiceSettlementDto>> GetInvoicesForSettlementAsync(List<Guid> invoiceIds);
        Task SyncSettlementBalancesAsync(IEnumerable<Guid> invoiceIds, IEnumerable<Guid> paymentDocIds, IEnumerable<Guid> detentionIds);
        //Task<IEnumerable<object>> GetDetentionsForSettlementAsync(List<Guid> detentionIds);
    }
}
