using CodeMatrix.Mepd.Application.Identity.RoleClaims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CodeMatrix.Mepd.Infrastructure.Auth.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRoleClaimsService _permissionService;

    public PermissionAuthorizationHandler(IRoleClaimsService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User?.GetUserId();
        if (userId is not null)
        {
            if (requirement.Operator == PermissionOperator.And)
            {
                foreach (var permission in requirement.Permissions)
                {
                    if (!await _permissionService.HasPermissionAsync(userId, permission))
                    {
                        // If the user lacks ANY of the required permissions
                        // we mark it as failed.
                        context.Fail();
                        return;
                    }
                }

                // identity has all required permissions
                context.Succeed(requirement);
                return;
            }

            foreach (var permission in requirement.Permissions)
            {
                if (await _permissionService.HasPermissionAsync(userId, permission))
                {
                    // In the OR case, as soon as we found a matching permission
                    // we can already mark it as Succeed
                    context.Succeed(requirement);
                    return;
                }
            }
        }

        // identity does not have any of the required permissions
        context.Fail();
    }
}