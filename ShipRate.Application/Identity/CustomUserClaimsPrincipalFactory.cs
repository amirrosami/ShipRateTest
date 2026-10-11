using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using System.Security.Claims;

namespace ShipRate.Application.Identity
{






    public class CustomUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>,
          ICustomUserClaimsPrincipalFactory
    {
        private readonly IPermissionService _permissionService;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IPermissionService permissionService)
            : base(userManager, roleManager, optionsAccessor)
        {
            _permissionService = permissionService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var permissionClaims = await _permissionService.GetUserPermissionClaimsAsync(user.Id);

            var distinctPermissionClaims = permissionClaims
                .Where(c => c.Type == AppClaimTypes.Permission)
                .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                .GroupBy(c => new { c.Type, c.Value })
                .Select(g => g.First())
                .ToList();

            identity.AddClaims(distinctPermissionClaims);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                identity.AddClaim(new Claim(AppClaimTypes.FirstName, user.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                identity.AddClaim(new Claim(AppClaimTypes.LastName, user.LastName));
            }

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                identity.AddClaim(new Claim(AppClaimTypes.FullName, user.FullName));
            }
            if (!string.IsNullOrWhiteSpace(user.JobTitle))
            {
                identity.AddClaim(new Claim(AppClaimTypes.JobTitle, user.JobTitle));
            }
            return identity;
        }
    }
    public static class AppClaimTypes
    {
        public const string Permission = "Permission";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string FullName = "FullName";
        public const string JobTitle = "JobTitle";
    }



}
