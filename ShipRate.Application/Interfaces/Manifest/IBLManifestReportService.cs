using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Manifest
{
    public interface IBLManifestReportService
    {
        Task<BLManifestReportVm?> GetAsync(Guid blId, CancellationToken cancellationToken = default);
        Task<BLDocumentReportVm?> GetBillOfLadingAsync(Guid blId, bool isOriginal, CancellationToken cancellationToken = default);
        Task<ContainerPackingListReportVm?> GetContainerPackingListAsync(Guid blId, CancellationToken cancellationToken = default);
        Task<CargoAllocationByContainerReportVm?> GetCargoAllocationByContainerAsync(Guid blId, CancellationToken cancellationToken = default);
        Task<byte[]?> GenerateSeaBillOfLadingPdfAsync(Guid blId, bool isOriginal, CancellationToken cancellationToken = default);
    }
}


