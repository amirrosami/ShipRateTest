using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IProductStockTransactionService
    {
        Task<PagedResultDto<ProductStockTransactionListDto>> GetPagedAsync(
            ProductStockTransactionFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<ProductStockTransactionCreateVm> GetCreateVmAsync(
            CancellationToken cancellationToken = default);

        Task CreateAsync(
            CreateProductStockTransactionDto input,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<InventoryProductStockDto>> GetInventoryStocksAsync(
            InventoryProductStockFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<List<SelectListItem>> GetInventorySelectListAsync(CancellationToken cancellationToken = default);
        Task<List<SelectListItem>> GetProductSelectListAsync(CancellationToken cancellationToken = default);

        Task<DeliverProductRequestVm> GetDeliverVmAsync(Guid productRequestId, CancellationToken cancellationToken = default);
        Task DeliverAsync(DeliverProductRequestDto input, Guid userId, CancellationToken cancellationToken = default);

    }
}
