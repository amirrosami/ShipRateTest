using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface IInvoiceAppService
    {
        Task<InvoiceListPageVm> GetListPageAsync(InvoiceListFilterVm filter, CancellationToken cancellationToken = default);
        Task<InvoiceEditVm> GetCreateVmAsync(Guid? jobId, Guid? blId, Guid? bookingId, CancellationToken cancellationToken = default, string FilterType = "All");
        Task<InvoiceEditVm?> GetEditVmAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> SaveHeaderAsync(InvoiceEditVm vm, CancellationToken cancellationToken = default);
        Task<bool> SoftDeleteInvoiceAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<InvoiceItemRowVm>> GetItemsAsync(Guid invoiceId, CancellationToken cancellationToken = default);
        Task<InvoiceItemEditVm> GetNewItemVmAsync(Guid invoiceId, CancellationToken cancellationToken = default);
        Task<InvoiceItemEditVm?> GetEditItemVmAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> SaveItemAsync(InvoiceItemEditVm vm, CancellationToken cancellationToken = default);
        Task SoftDeleteItemAsync(Guid id, CancellationToken cancellationToken = default);

        Task<InvoiceTotalsVm> GetTotalsAsync(Guid invoiceId, CancellationToken cancellationToken = default);

        Task<PagedResultVm<PartyLookupRowVm>> GetPartyLookupAsync(PartyLookupFilterVm filter, Guid? jobId, CancellationToken cancellationToken = default);
        Task<List<LookupItemVm>> GetChargesForItemAsync(int? currencyId, int? chargeTypeId, int? financeDirectionId, CancellationToken cancellationToken = default);
        Task<string> GetInvoiceNoAsync(Guid invoiceId);
        Task<int> GetInvoiceCurrencyIdAsync(Guid invoiceId);
        Task<ServiceResult<CreateOnlinePaymentVm>> GetNewOnlinePayment(List<Guid> invoiceIds);

        Task<int> SaveInvoiceDOAsync(InvoiceEditVm vm, CancellationToken cancellationToken);
        Task<bool> ExistsSaleDoInvoiceForBLAsync(Guid? bLId);
        Task<byte[]> GenerateOfficialTaxInvoicePdfAsync(InvoicePrintDto invoiceDto, CancellationToken cancellationToken = default);
        Task<InvoicePrintDto> GetInvoicePrintDtoAsync(Guid invoiceId);
        Task<int> GetNextSerialAsync(int CompanyId, string serialPrefix, CancellationToken ct = default);
        Task SetInvoiceSerialAsync(InvoiceEditVm vm);
        Task<(bool canBeDeleted, string message)> InvoiceCanBeDeleted(Guid id, CancellationToken cancellationToken = default);
    }
}
