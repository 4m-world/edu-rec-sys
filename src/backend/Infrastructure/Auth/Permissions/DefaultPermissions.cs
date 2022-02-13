using CodeMatrix.Mepd.Shared.Authorization;

namespace CodeMatrix.Mepd.Infrastructure.Auth.Permissions;

public static class DefaultPermissions
{
    public static List<string> Basics => new()
    {
        MepdPermissions.UserProfile.View,
        MepdPermissions.UserProfile.ChangePassword,
        MepdPermissions.Users.ViewOwn,
    };
}