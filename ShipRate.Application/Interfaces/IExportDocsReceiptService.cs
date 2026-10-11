using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IExportDocsReceiptService
    {
        Task<PagedResultDto<ExportDocsReceiptListItemVm>> GetPagedAsync(ExportDocsReceiptIndexVm input);
        Task<ExportDocsReceiptFormVm> GetCreateModelAsync(Guid? bookingId);
        Task<ExportDocsReceiptFormVm?> GetEditModelAsync(Guid id);

        Task<Guid> CreateAsync(ExportDocsReceiptFormVm input);
        Task UpdateAsync(ExportDocsReceiptFormVm input);
        Task DeleteAsync(Guid id);

        Task<AddExportDocsReceiptDocumentVm> GetAddDocumentModelAsync(Guid exportDocsReceiptId);
        Task AddDocumentAsync(AddExportDocsReceiptDocumentVm input);
        Task RemoveDocumentAsync(Guid exportDocsReceiptItemId);

        Task<AddExportDocsReceiptContainerVm> GetAddContainerModelAsync(Guid exportDocsReceiptId);
        Task AddContainerAsync(AddExportDocsReceiptContainerVm input);
        Task RemoveContainerAsync(Guid exportDocsReceiptContainerId);
        Task<bool> ConfirmAsync(Guid id);
    }
}
