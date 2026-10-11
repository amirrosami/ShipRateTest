using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface IPettyCashService
    {
        Task<PagedResultDto<PettyCashListItemDto>> GetPettyCashesAsync(PettyCashIndexSearchDto search, CancellationToken cancellationToken = default);

        Task<PettyCashUpsertDto> GetPettyCashForCreateAsync(CancellationToken cancellationToken = default);
        Task<PettyCashUpsertDto> GetPettyCashForEditAsync(Guid id, CancellationToken cancellationToken = default);
        Task SavePettyCashAsync(PettyCashUpsertDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeletePettyCashAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PettyCashExpenseListItemDto>> GetExpensesAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<PettyCashExpenseUpsertDto> GetExpenseForCreateAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<PettyCashExpenseUpsertDto> GetExpenseForEditAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveExpenseAsync(PettyCashExpenseUpsertDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteExpenseAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PettyCashRechargeListItemDto>> GetRechargesAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<PettyCashRechargeUpsertDto> GetRechargeForCreateAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<PettyCashRechargeUpsertDto> GetRechargeForEditAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveRechargeAsync(PettyCashRechargeUpsertDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteRechargeAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<PettyCashLookupDto> GetLookupsAsync(int? companyId = null, CancellationToken cancellationToken = default);
        Task RejectExpenseAsync(Guid id);
        Task ApproveExpenseAsync(Guid id);
        Task ApproveRechargeAsync(Guid id);
        Task RejectRechargeAsync(Guid id);
        Task ChargeRechargeAsync(Guid id);
    }
}
