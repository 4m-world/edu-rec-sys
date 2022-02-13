namespace CodeMatrix.Mepd.Shared.Authorization;

public partial class RootPermissions
{
    public static class Tenants
    {
        public const string View = "Permissions.Tenants.View";
        public const string ListAll = "Permissions.Tenants.ViewAll";
        public const string Create = "Permissions.Tenants.Register";
        public const string Update = "Permissions.Tenants.Update";
        public const string UpgradeSubscription = "Permissions.Tenants.UpgradeSubscription";
        public const string Remove = "Permissions.Tenants.Remove";
    }

    public static class SystemInformation
    {
        public const string View = "Permissions.SystemInformation.View";
    }
}