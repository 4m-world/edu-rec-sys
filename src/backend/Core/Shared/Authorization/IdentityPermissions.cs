using System.ComponentModel;

namespace CodeMatrix.Mepd.Shared.Authorization;

public partial class MepdPermissions
{
    /// <summary>
    /// Identity Permissions
    /// </summary>
    [DisplayName("Identity")]
    [Description("Identity Permissions")]
    public static class Identity
    {
        public const string Register = "Permission.Identity.Register";
    }

    /// <summary>
    /// Roles permissions
    /// </summary>
    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        /// <summary>
        /// Permission to view a recrod
        /// </summary>
        public const string View = "Permission.Roles.View";

        /// <summary>
        /// Permission to view all recrod
        /// </summary>
        public const string ViewAll = "Permission.Roles.ViewAll";

        /// <summary>
        /// Permission to create a recrod
        /// </summary>
        public const string Register = "Permission.Roles.Create";

        /// <summary>
        /// Permission to Update a recrod
        /// </summary>
        public const string Update = "Permission.Roles.Edit";

        /// <summary>
        /// Permission to delete a recrod
        /// </summary>
        public const string Remove = "Permission.Roles.Delete";
    }

    /// <summary>
    /// Role Claims permissions
    /// </summary>
    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        /// <summary>
        /// Permission to view a recrod
        /// </summary>
        public const string View = "Permission.RoleClaims.View";

        /// <summary>
        /// Permission to create a recrod
        /// </summary>
        public const string Create = "Permission.RoleClaims.Create";

        /// <summary>
        /// Permission to Update a recrod
        /// </summary>
        public const string Edit = "Permission.RoleClaims.Edit";

        /// <summary>
        /// Permission to delete a recrod
        /// </summary>
        public const string Delete = "Permission.RoleClaims.Delete";

        /// <summary>
        /// Permission to search through recrods
        /// </summary>
        public const string Search = "Permission.RoleClaims.Search";
    }

    /// <summary>
    /// Users permissions
    /// </summary>
    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        /// <summary>
        /// Permission to view own recrod
        /// </summary>
        public const string ViewOwn = "Permission.Users.ViewOwn";
        
        /// <summary>
        /// Permission to view a recrod
        /// </summary>
        public const string View = "Permission.Users.View";

        /// <summary>
        /// Permission to view all recrod
        /// </summary>
        public const string ViewAll = "Permission.Users.ViewAll";

        /// <summary>
        /// Permission to create a recrod
        /// </summary>
        public const string Create = "Permission.Users.Create";

        /// <summary>
        /// Permission to Update a recrod
        /// </summary>
        public const string Edit = "Permission.Users.Edit";

        /// <summary>
        /// Permission to delete a recrod
        /// </summary>
        public const string Delete = "Permission.Users.Delete";

        /// <summary>
        /// Permission to export records
        /// </summary>
        public const string Export = "Permission.Users.Export";

        /// <summary>
        /// Permission to search through recrods
        /// </summary>
        public const string Search = "Permission.Users.Search";
    }

    /// <summary>
    /// User profile permissions
    /// </summary>
    [DisplayName("User Profile")]
    [Description("User Profile Permissions")]
    public static class UserProfile
    {
        /// <summary>
        /// Permission to change password
        /// </summary>
        public const string ChangePassword = "Permission.UserProfile.ChangePassword";

        /// <summary>
        /// Permission to view
        /// </summary>
        public const string View = "Permission.UserProfile.View";

        /// <summary>
        /// Permission to update
        /// </summary>
        public const string Edit = "Permission.UserProfile.Edit";
    }
}