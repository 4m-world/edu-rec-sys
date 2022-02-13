using System.ComponentModel;

namespace CodeMatrix.Mepd.Shared.Authorization;

public partial class MepdPermissions
{
    /// <summary>
    /// Audit trail logs permissions
    /// </summary>
    [DisplayName("Audit Trail Logs")]
    [Description("Audit Trail Logs Permissions")]
    public static class AuditLogs
    {
        /// <summary>
        /// Permission to view records
        /// </summary>
        public const string View = "Permission.AuditLogs.View";

        /// <summary>
        /// Permission to view user records only
        /// </summary>
        public const string ViewOwn = "Permission.AuditLogs.ViewOwn";

        /// <summary>
        /// Permission to export records
        /// </summary>
        public const string Export = "Permission.AuditLogs.Excport";
    }
}
