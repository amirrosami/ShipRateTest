using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Finance
{

    public interface IAccountCodeService
    {
        Task<PagedResultDto<AccountCodeListItemVm>> GetPagedAsync(AccountCodeSearchVm input);
        Task<AccountCodeCreateEditVm?> GetByIdForEditAsync(Guid id);
        Task<AccountCodeCreateEditVm> GetCreateVmAsync();
        Task<AccountCodeCreateEditVm> FillLookupsAsync(AccountCodeCreateEditVm vm);

        Task<(bool Succeeded, string Message)> CreateAsync(AccountCodeCreateEditVm vm, string? userId);
        Task<(bool Succeeded, string Message)> UpdateAsync(AccountCodeCreateEditVm vm, string? userId);
        Task<(bool Succeeded, string Message)> DeleteAsync(Guid id, string? userId);
    }
}
