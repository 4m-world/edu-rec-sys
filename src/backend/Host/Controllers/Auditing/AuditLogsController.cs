using CodeMatrix.Mepd.Application.Auditing;

namespace CodeMatrix.Mepd.Host.Controllers.Auditing;

/// <summary>
/// Audit logs api controller
/// </summary>
[Route("api/audit-logs")]
public class AuditLogsController : VersionNeutralApiController
{
    /// <summary>
    /// Get current user logs
    /// </summary>
    [HttpGet]
    [MustHavePermission(MepdPermissions.AuditLogs.View)]
    public Task<PaginationResponse<AuditDto>> GetMyLogsAsync(string search, int pageNumber, int pageSize = 10, [FromQuery] string[] orderBy = default)
    {
        return Mediator.Send(new GetMyAuditLogsRequest { Keyword = search, PageNumber = pageNumber, PageSize = pageSize, OrderBy = orderBy });
    }
}