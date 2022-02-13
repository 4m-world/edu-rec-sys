using CodeMatrix.Mepd.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CodeMatrix.Mepd.Infrastructure.Identity.Extensions;

public static class RoleManagerExtensions
{
    public static async Task<IdentityResult> AddPermissionClaimAsync(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == MepdClaims.Permission && a.Value == permission))
        {
            return await roleManager.AddClaimAsync(role, new Claim(MepdClaims.Permission, permission));
        }

        return IdentityResult.Failed();
    }
}