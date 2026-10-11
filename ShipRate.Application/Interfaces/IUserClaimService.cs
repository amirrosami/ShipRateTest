using ShipRate.Domain.Entities.Security;

namespace ShipRate.Application.Interfaces
{
    public interface IUserClaimService
    {
        Task RefreshUserPermissionClaimsAsync(ApplicationUser user);
    }
}
