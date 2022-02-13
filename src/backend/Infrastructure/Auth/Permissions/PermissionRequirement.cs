using CodeMatrix.Mepd.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CodeMatrix.Mepd.Infrastructure.Auth.Permissions;

internal class PermissionRequirement : IAuthorizationRequirement
{
    public static string CliamType => MepdClaims.Permission;

    public PermissionOperator Operator { get; }

    public string[] Permissions { get; }

    public PermissionRequirement(PermissionOperator permissionOperator, string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            throw new ArgumentNullException("At least one permission is required", nameof(permissions));

        Permissions = permissions;
        Operator = permissionOperator;
    }

    public PermissionRequirement(string permission)
        : this(PermissionOperator.And, new string[] { permission }) { }
}

public enum PermissionOperator
{
    And,
    Or
}