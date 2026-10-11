using Microsoft.AspNetCore.Identity;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using System.Security.Claims;

namespace ShipRate.Application.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;

        public UserClaimService(
            UserManager<ApplicationUser> userManager,
            IPermissionService permissionService)
        {
            _userManager = userManager;
            _permissionService = permissionService;
        }

        public async Task RefreshUserPermissionClaimsAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            const string PermissionClaimType = "Permission";

            var existingClaims = await _userManager.GetClaimsAsync(user);

            var oldPermissionClaims = existingClaims
                .Where(c => c.Type == PermissionClaimType)
                .ToList();

            foreach (var claim in oldPermissionClaims)
            {
                var removeResult = await _userManager.RemoveClaimAsync(user, claim);

                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(" | ", removeResult.Errors.Select(e => e.Description)));
                }
            }
            var permissions = await _permissionService.GetEffectivePermissionsAsync(user.Id);
            var newPermissionClaims = permissions
                .Select(p => p.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .Select(name => new Claim(PermissionClaimType, name))
                .ToList();

            if (newPermissionClaims.Any())
            {
                var addResult = await _userManager.AddClaimsAsync(user, newPermissionClaims);

                if (!addResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(" | ", addResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
