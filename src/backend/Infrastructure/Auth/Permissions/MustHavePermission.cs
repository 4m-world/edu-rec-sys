using Microsoft.AspNetCore.Authorization;

namespace CodeMatrix.Mepd.Infrastructure.Auth.Permissions;

public class MustHavePermission : AuthorizeAttribute
{
    internal const string PolicyPrefix = "PERMISSION_";
    private const string Separator = "_";

    public MustHavePermission(PermissionOperator permissionOperator, params string[] permissions)
    {
        Policy = $"{PolicyPrefix}{(int)permissionOperator}{Separator}{string.Join(Separator, permissions)}";
    }
    public MustHavePermission(string permission)
        : this(PermissionOperator.And, permission)
    {
    }

    public static PermissionOperator GetOperatorFromPolicy(string policyName)
    {
        var @operator = int.Parse(policyName.AsSpan(PolicyPrefix.Length, 1));
        return (PermissionOperator)@operator;
    }

    public static string[] GetPermissionsFromPolicy(string policyName)
    {
        return policyName.Substring(PolicyPrefix.Length + 2)
               .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
    }
}